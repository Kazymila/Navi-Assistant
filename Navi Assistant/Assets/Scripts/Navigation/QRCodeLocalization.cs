using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Localization;
using Unity.XR.CoreUtils;
using UnityEngine.Events;
using Unity.Collections;
using UnityEngine.UI;
using UnityEngine;
using ZXing;
using TMPro;

public class QRCodeLocalization : MonoBehaviour
{
    [Header("AR Components")]
    [SerializeField] private ARSession _session;
    [SerializeField] private XROrigin _sessionOrigin;
    [SerializeField] private ARCameraManager _cameraManager;

    [Header("External References")]
    [SerializeField] private AnalyticsDataManager _analyticsManager;
    [SerializeField] private GameObject _assistantUI;
    [SerializeField] private NavigationManager _navManager;
    [SerializeField] private PopUpAlertController _alertPanel;

    [Header("Scanner Components")]
    [SerializeField] private GameObject _qrCodeScannerPanel;
    [SerializeField] private TextMeshProUGUI _qrCodeTextDisplay;
    [SerializeField] private RectTransform _scanZone;
    [SerializeField] private Button _backButton;

    [Header("Messages")]
    [SerializeField] private LocalizedString _invalidQrCodeMessage;
    [SerializeField] private LocalizedString _localizedAlertMessage;

    private IBarcodeReader _reader = new BarcodeReader();
    private Texture2D _cameraImageTexture;
    private bool _scanningEnabled = true;
    private UnityEvent _onCodeLocalized;

    private void OnEnable()
    {
        _scanningEnabled = true;
        _qrCodeScannerPanel.SetActive(true);
        _cameraManager.frameReceived += OnCameraFrameReceived;
        _qrCodeTextDisplay.text = "";

#if UNITY_EDITOR
        Invoke("TestLocalization", 1f); // Test localization in the editor
#endif
    }

    private void TestLocalization()
    {   // Test location on the XROrigin position (change the XROrigin position to test different locations)
        GetQrCodeLocalization($"{_sessionOrigin.transform.position}pos:dir{_sessionOrigin.transform.forward}");
    }

    private void OnDisable()
    {
        _scanningEnabled = false;
        _qrCodeScannerPanel.SetActive(false);
        _cameraManager.frameReceived -= OnCameraFrameReceived;
    }

    private void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
    {   // Process the camera frame to detect QR code
        if (!_scanningEnabled) return;
        if (!_cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image)) return;

        var conversionParams = new XRCpuImage.ConversionParams
        {
            inputRect = new RectInt(0, 0, image.width, image.height),              // Get the full image
            outputDimensions = new Vector2Int(image.width / 2, image.height / 2), // Downsample by 2
            outputFormat = TextureFormat.RGBA32,                                 // Choose RGBA format
            transformation = XRCpuImage.Transformation.MirrorY // Flip across the vertical axis (mirror image)
        };
        // Get bytes to store the final image and allocate memory for it
        int size = image.GetConvertedDataSize(conversionParams);
        var buffer = new NativeArray<byte>(size, Allocator.Temp);

        image.Convert(conversionParams, buffer); // Extract the image data

        // The image was converted to RGBA32 format and written into the provided buffer
        image.Dispose(); // Dispose the XRCpuImage to avoid resource leaks

        // Put the image into a texture
        _cameraImageTexture = new Texture2D(
            conversionParams.outputDimensions.x,
            conversionParams.outputDimensions.y,
            conversionParams.outputFormat,
            false);

        _cameraImageTexture.LoadRawTextureData(buffer);
        _cameraImageTexture.Apply();
        buffer.Dispose(); // Dispose the buffer to avoid memory leaks

        // Detect and decode the barcode inside the bitmap
        var result = _reader.Decode(
            _cameraImageTexture.GetPixels32(),
            _cameraImageTexture.width,
            _cameraImageTexture.height
            );

        if (result != null)
        {   // Display the QR code text and localize the device
            GetQrCodeLocalization(result.Text);
        }
    }

    private void GetQrCodeLocalization(string _qrCodeText)
    {   // Get the localization of user's device based on QR code

        if (_qrCodeText.Contains("pos:dir"))
        {   // Split the QR code text to get position and rotation
            string[] _QRSplit = _qrCodeText.Split(")pos:dir(");
            string _QRSplitPosition = _QRSplit[0].Replace("(", "");
            string _QRSplitDirection = _QRSplit[1].Replace(")", "");

            string[] _QRpos = _QRSplitPosition.Split(',');
            string[] _QRdir = _QRSplitDirection.Split(',');

            // Split the position and rotation to get x, y, z values
            if (_QRpos.Length != 3 || _QRdir.Length != 3) return;

            Vector3 QRPosition = new Vector3(
                float.Parse(_QRpos[0], System.Globalization.CultureInfo.InvariantCulture),
                float.Parse(_QRpos[1], System.Globalization.CultureInfo.InvariantCulture),
                float.Parse(_QRpos[2], System.Globalization.CultureInfo.InvariantCulture));
            Vector3 QRDirection = new Vector3(
                float.Parse(_QRdir[0], System.Globalization.CultureInfo.InvariantCulture),
                float.Parse(_QRdir[1], System.Globalization.CultureInfo.InvariantCulture),
                float.Parse(_QRdir[2], System.Globalization.CultureInfo.InvariantCulture));

            // Reset position and rotation of ARSession
            _session.Reset();

            // Add offset for recentering
            _sessionOrigin.transform.position = QRPosition;
            _sessionOrigin.transform.rotation = Quaternion.LookRotation(
                QRDirection, _sessionOrigin.transform.up);

            _onCodeLocalized.Invoke();
        }
        else
        {   // Display an error message if the QR code format is invalid
            _qrCodeTextDisplay.text = _invalidQrCodeMessage.GetLocalizedString();
            Debug.Log("[Localization System] Invalid QR code format");
        }
    }

    public void ChangeLocalizedAction(UnityEvent _newAction)
    {   // Change the action when the QR code is localized
        _onCodeLocalized = new UnityEvent();
        _onCodeLocalized = _newAction;
    }

    public void ChangeBackButtonAction(Button.ButtonClickedEvent _newAction)
    {   // Change the action of the back button
        _backButton.onClick.RemoveAllListeners();
        _backButton.onClick = _newAction;
    }

    public void ResetScannerButtonsActions()
    {   // Reset the actions of the scanner buttons
        UnityEvent _onLocalized = new UnityEvent();
        Button.ButtonClickedEvent _onBack = new Button.ButtonClickedEvent();

        _onLocalized.AddListener(() =>
        {
            _assistantUI.SetActive(true);
            _navManager.StartNavigation();
            _alertPanel.ShowTimingAlert(_localizedAlertMessage.GetLocalizedString(), 1f);
            _analyticsManager.analyticsData.QRrelocalizationCount++;
            this.gameObject.SetActive(false);
        });

        _onBack.AddListener(() =>
        {
            _assistantUI.SetActive(true);
            _navManager.StartNavigation();
            this.gameObject.SetActive(false);
        });

        ChangeLocalizedAction(_onLocalized);
        ChangeBackButtonAction(_onBack);
    }

    public void ToggleScanning()
    {   // Enable/Disable QR code scanning
        _scanningEnabled = !_scanningEnabled;
        _qrCodeScannerPanel.SetActive(_scanningEnabled);
    }
}

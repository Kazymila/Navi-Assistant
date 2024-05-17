using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;
using Unity.Collections;
using UnityEngine;
using ZXing;
using TMPro;
using Unity.VisualScripting;

public class QRCodeLocalization : MonoBehaviour
{
    [Header("AR Components")]
    [SerializeField] private ARSession _session;
    [SerializeField] private XROrigin _sessionOrigin;
    [SerializeField] private ARCameraManager _cameraManager;

    [Header("Scanner Components")]
    [SerializeField] private GameObject _qrCodeScannerPanel;
    [SerializeField] private TextMeshProUGUI _qrCodeText;
    [SerializeField] private RectTransform _scanZone;

    private IBarcodeReader _reader = new BarcodeReader(); // create a barcode reader instance
    private Texture2D _cameraImageTexture;
    private bool _scanningEnabled = true;

    private void OnEnable()
    {
        _scanningEnabled = true;
        _cameraManager.frameReceived += OnCameraFrameReceived;
    }

    private void OnDisable()
    {
        _scanningEnabled = false;
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
        var result = _reader.Decode(_cameraImageTexture.GetPixels32(), _cameraImageTexture.width, _cameraImageTexture.height);

        if (result != null)
        {   // Display the QR code text and localize the device
            _qrCodeText.text = result.Text;
            GetQrCodeLocalization(result.Text);
            //_qrCodeScanningPanel.SetActive(false);
        }
    }

    private void GetQrCodeLocalization(string _qrCodeText)
    {   // Get the localization of user's device based on QR code

        if (_qrCodeText.Contains(":pos:") && _qrCodeText.Contains(":rot:"))
        {   // Split the QR code text to get position and rotation
            string[] _QRSplit = _qrCodeText.Split(":pos:");
            string _QRSplitPosition = _QRSplit[1].Split(":rot:")[0];
            string _QRSplitRotation = _QRSplit[1].Split(":rot:")[1];
            string _QRLabel = _QRSplit[0];

            // Split the position and rotation to get x, y, z values
            string[] _QRpos = _QRSplitPosition.Split(',');
            string[] _QRrot = _QRSplitRotation.Split(',');

            if (_QRpos.Length != 3 || _QRrot.Length != 3) return;
            Vector3 QRPosition = new Vector3(float.Parse(_QRpos[0]), float.Parse(_QRpos[1]), float.Parse(_QRpos[2]));
            Vector3 QRRotation = new Vector3(float.Parse(_QRrot[0]), float.Parse(_QRrot[1]), float.Parse(_QRrot[2]));

            // Reset position and rotation of ARSession
            _session.Reset();

            // Add offset for recentering
            _sessionOrigin.transform.position = QRPosition;
            _sessionOrigin.transform.rotation = Quaternion.Euler(QRRotation);
        }
    }

    public void ToggleScanning()
    {   // Enable/Disable QR code scanning
        _scanningEnabled = !_scanningEnabled;
        _qrCodeScannerPanel.SetActive(_scanningEnabled);
    }
}

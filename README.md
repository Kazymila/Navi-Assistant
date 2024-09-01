> <img src="https://github.com/user-attachments/assets/4063f705-e670-441b-822b-5392f1f51b68" align="right" width=65/>
# Navi Assistant
 Augmented Reality (AR) Virtual Assistant for Indoor Navigation

 ![navi](https://github.com/user-attachments/assets/5e8400ca-f126-4b81-9ee2-3106d41dd119)

Made in Unity 2022.3.1f1, using Google ARCore and Firebase services

## :notebook: About
This is a prototype of a mobile application for indoor navigation in augmented reality using a virtual assistant. This navigation system uses QR codes to locate the user in the indoor environment. For this purpose, this system relies on the [Navi Admin](https://github.com/Kazymila/Navi-Admin) management platform, where you can model an indoor environment, configure its spaces and generate the QR codes.

*The mobile application and the management platform are prototypes developed as an undergraduate thesis project to obtain the degree of Civil Engineering in Computer Science at the Universidad de O'Higgins, Rancagua, Chile*

The application was tested in two indoor environments, inside a house and at the University of O'Higgins. In the following demo videos you can see how the application works in both cases.

<div align="center">
    <table >
     <tr>
        <td><b>Indoor House Demo</b></td>
        <td><b>UOH Building Demo</b></td>
     </tr>
     <tr>
       <td><video src="https://github.com/user-attachments/assets/51d1cf65-b99f-44e6-ba6c-fac31ec82f8a"/></td>
        <td><video src="https://github.com/user-attachments/assets/c6733ec8-1d51-400a-b802-9b2ae7cc2904"/></td>
     </tr>
    </table>
</div>

## :clipboard: Instructions
When you open the project Unity will install all the dependencies, but you will see a error because need some extra Firebase packages. You can open the project on save mode and install the packages how is shown on the [Firebase docs](https://firebase.google.com/docs/unity/setup).

> [!NOTE]
> You only need the Firestore package from the SDK. Also, you need to generate Firebase credentials for an Android app and save the `google-services.json` file in a folder called `StreamingAssets` inside the Assets folder.

However, you can directly open the project to test the aplication with local map files. This files are JSON format and can be generated in [Navi Admin](https://github.com/Kazymila/Navi-Admin), where you can create an indoor map that can use in this app!

The [Assets/MapTests](Navi%20Assistant/Assets/MapTests) folder have two map files examples that you can test on the editor. To test a local file you need to:
<img src="https://github.com/user-attachments/assets/519aa174-705b-4254-99ca-a4002d37a3ba" align="right"/>
* Save the file on the [MapTests](Navi%20Assistant/Assets/MapTests) folder in the project
* Go to the `MapLoader` Gameobject in hierarchy to change the map local file name on the inspector
* Also you need to check the "Load Local File" checkbox

> [!IMPORTANT]
> If you uncheck the "Load Local File" option, the app will try to load the map from Firestore. If you have installed it and have credentials, you can upload the map file from Navi Admin to the server and then will load in the app.
>
> *(The map document name field corresponds to the name of the document where is saved the data on the Firestore database)*
>
> ***This is only necessary to export the application to Smartphones.***

<img src="https://github.com/user-attachments/assets/f2faa9b9-add6-4775-894b-b2023d48d91c" align="right"/>
Before testing the application in the Unity editor it is also necessary to position where the navigation starts or where the user will scan the QR code. For this you can move the object called XROrigin that represents the user in the virtual space to test different locations or move the object to see how the application will behave as the user moves through the environment.

---

Then press play in the Unity editor and the app will work with the loaded map. As shown here:

https://github.com/user-attachments/assets/1ede7f6a-f2bb-40ea-93cb-c4a57d52b53c


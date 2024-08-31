> <img src="https://github.com/user-attachments/assets/4063f705-e670-441b-822b-5392f1f51b68" align="right" width=65/>
# Navi Assistant
 Augmented Reality (AR) Virtual Assistant for Indoor Navigation

 ![navi](https://github.com/user-attachments/assets/5e8400ca-f126-4b81-9ee2-3106d41dd119)

Made in Unity 2022.3.1f1, using Google ARCore and Firebase services

## :notebook: About
This is a prototype of a mobile application for indoor navigation in augmented reality using a virtual assistant. 

The application was tested in two indoor environments, inside a house and at the University of O'Higgins, Rancagua, Chile. In the following demo videos you can see how the application works in both cases.

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

> [!IMPORTANT]
> You only need the Firestore package from the SDK. Also, you need to generate Firebase credentials for an Android app and save the `google-services.json` file in a folder called `StreamingAssets` inside the Assets folder.

However, you can directly open the project to test the aplication with local map files. This files are JSON format and can be generated in the administration software: [Navi Assistant](https://github.com/Kazymila/Navi-Admin), where you can create an indoor map that can use in this app!

The [Assets/MapTests](Navi%20Assistant/Assets/MapTests) folder have two map files examples that you can test on the editor. To test a local file, you need to:
* Save the file on the [MapTests](Navi%20Assistant/Assets/MapTests) folder in the project
* Go to the `MapLoader` Gameobject in hierarchy to change the local map file name on the inspector
* Also you need to check the "load local file" checkbox

> [!IMPORTANT]
> If you uncheck the "load local file" option, the app will try to load the map from Firestore. If you have installed it and have credentials, you can upload the map file from Navi Admin to the server and then will load in the app.
>
> *This is only necessary to export the application to Smartphones.*

Then press play in the Unity editor and the app will work with the loaded map. As shown here:

https://github.com/user-attachments/assets/1ede7f6a-f2bb-40ea-93cb-c4a57d52b53c


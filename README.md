# ABPHelper
ABPHelper is a Visual Studio Extension(VSIX) that helps you with developing ASP.NET Boilerplate applications.
![](https://github.com/wakuflair/ABPHelper/blob/master/images/abphelper2.png?raw=true)

## Installation
You can install **ABPHelper** by using **Extensions and Updates** in Visual Studio. Or download it from [Visual Studio Gallary](https://visualstudiogallery.msdn.microsoft.com/15d33189-e63e-4ab4-9269-bc43200d7836) and install it manullay.
![](https://github.com/wakuflair/ABPHelper/blob/master/images/abphelper1.png?raw=true)

## Using
Once you installed **ABPHelper**, you can find it in **Views**->**Other Windows**->**ABPHelper**.

By now, **ABPHelper** has one feature:

- **Generate ApplicationService Methods**

	Open an ApplicationService source code, then input methods names you want to generate in **Method Names** textbox(one name per line), check **Async Methods** if you wish, then click **Generate** button, **ABPHelper** will do following things:

	- All methods will be generated in current ApplicationService class file. 
	- All methods will be generated in corresponding Interface file.
	- All DTO files will be created and added to `Dto` folder of current project. 

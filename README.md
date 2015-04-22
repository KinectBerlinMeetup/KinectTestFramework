# KinectTestFramework
This is an open source Repository with an approach of a Test framework for Kinect v2 Development
TODO: Write a project description
* Add some Kittens

## Requirements
* Kinect for Windows SDK 2.0
* Kinect for Windows Sensor v2
* 

## Using KinectStudio API in Visual Studio 13 
1. Go to the installation Folder of the KinectStudio (in my case: C:\Program Files\Microsoft SDKs\Kinect\v2.0_1409\Redist\KinectStudio­) TODO: check if the Path is Correct!
2. Reference "Microsoft.Kinect.Tools.dll" in your Projekt (This is not the Microsoft.Kinect.Toolkit Library that is found elsewhere)
3. Copy "KStudioService.dll" in your debug or release folder
4. Change BuildProperties to x64 (otherwise I got a BadImageFormatException)

## Links
* https://social.msdn.microsoft.com/Forums/en-US/59c97d1e-79f6-4dd0-8fae-73080a2c7b18/documentation-for-microsoftkinecttools-api?forum=kinectv2sdk forum entry to setup API for usage
* https://msdn.microsoft.com/en-us/library/microsoft.kinect.tools.aspx API page

## Usage
* Help us to build a testframework based on the KinectStudio API
TODO: Write usage instructions

## Contributing

1. Fork it!
2. Create your feature branch: `git checkout -b my-new-feature`
3. Commit your changes: `git commit -am 'Add some feature'`
4. Push to the branch: `git push origin my-new-feature`
5. Submit a pull request :D

## Credits

TODO: Write credits

## License

TODO: Write license -> MIT stuff?

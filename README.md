# Json Importer
This package adds a custom editor for `.json` files. The default Unity behaviour is treat `.json` files the same as all TextAssets; rendering the contents as one long label. This package improves upon this by adding formatting to the editor.

## Before & After
<p align="center">
<img src="https://user-images.githubusercontent.com/85991229/138490047-97ee38d3-856e-4a0f-a601-6ae7820cd057.png" width="300" align="center" /><img src="https://user-images.githubusercontent.com/85991229/138490139-31a9819b-d96f-4bc3-b035-12d8927b352d.png" width="300"  align="center" />
</p>

## Summary
☑️This package will:
- Improve legibility of json files within the editor
- Change the way `.json` file preview is rendered in Unity
- Add interactive UI elements to the preview


❎ This package will not
- Edit the source file of the `.json` file
- Add functionality to alter json object
- Add any runtime code

## Install
1. Within Unity navigate to Window>Package Manager
2. Click the +
3. Select `Add package from git URL...`
4. Enter https://github.com/MrBinaryCats/JsonImporter.git

Once installed any `.json` files should use new json importer.

## Examples
Here are some examples
### Simple Types
```json
{
  "stringField": "1",
  "intField": "int",
  "boolField": true
}
```
![image](https://user-images.githubusercontent.com/85991229/138494034-b29b63fc-d7e1-4645-96e6-9ba4221ca57d.png)

### Objects
Objects appear in foldouts which you can collapse/expand 
```json
{
  "obj1": {
    "stringField": "string"
  },
  "obj2": {
    "intField": 1,
    "subObj2": {},
    "subObj3": {
      "boolField": true
    }
  }
}
```
![image](https://user-images.githubusercontent.com/85991229/138493552-650258d4-44ec-4f57-b9d5-fe5fcfd66f6d.png)

### Arrays
```json
{
  "array": [
    "string",
   {
    "intField": 1,
    "subObj2": {},
    "subObj3": {
      "boolField": true
      }
    }
  ]
}
```
Similar to objects, arrays elements are collapsable
![image](https://user-images.githubusercontent.com/85991229/138493195-04373e4b-d949-4b08-a45d-72dd87fd39f4.png)


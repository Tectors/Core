<div align="center">
   <img src="https://github.com/user-attachments/assets/149ad432-c644-4692-b14f-39eb73048a91" width="126" height="126"/>
</div>

</br>
   <div align="center">
      
   [![Discord](https://img.shields.io/badge/Join%20Discord-Collector?color=black&logo=discord&logoColor=white&style=for-the-badge)](https://discord.gg/eV9DF6sBsz)
   [![Buy Me a Coffee](https://img.shields.io/badge/Buy%20Me%20a%20Coffee-Ko--fi?color=black&logo=ko-fi&logoColor=white&style=for-the-badge)](https://ko-fi.com/t4ctor)
   [![Releases](https://img.shields.io/github/downloads/Tectors/Core/total?style=for-the-badge&color=black&label=Downloads)](https://github.com/Tectors/Core/releases)
</div>

### Description
Core is an **extension** of [`JsonAsAsset`](https://github.com/JsonAsAsset/JsonAsAsset) to fetch almost every referenced asset for you hands-free.

### Licensing

This project is licensed under the [Apache 2.0 License](./Licensing/Apache-2.0.txt). We make use of open-source libraries created by many talented developers.

For detailed license information and third-party attributions, please refer to:
* The [Licensing directory](./Licensing)
* The [THIRD\_PARTY\_NOTICES.md](./Licensing/THIRD_PARTY_NOTICES.md) file

To simplify distribution, all license texts have been consolidated into a single [NOTICE](./Licensing/NOTICE) file.

-----------------

### **Table of Contents**

> 1. [Installation](#installation)  
> 2. [Git Commands](#commands)

-----------------

<a name="installation"></a>
## How to Use

1. **Visit the Releases Page:**  
   Go to the [Releases page](/../../releases), and download the latest release.
2. **Setup the Application:**  
   Follow the setup process.
3. **Start a new Profile:**  
   Go to the Profiles tab (bottom left) and load a profile, or create one.
4. **Profit.**  

<a name="commands"></a>
## Git Commands
#### Clone
```
git clone https://github.com/Tectors/Core --recursive
```
#### Build
```
dotnet publish Source/vj0 -c Release --no-self-contained -r win-x64 -o "./Release" -p:PublishSingleFile=true -p:DebugType=None -p:DebugSymbols=false -p:IncludeNativeLibrariesForSelfExtract=true
```
#### Update Libraries
```
Dependencies\update_libraries.bat
```
#### Update Lisence After Changes
```
python .github/Licensing/main.py
```

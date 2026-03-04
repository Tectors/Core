<div align="center">

<img src="https://github.com/user-attachments/assets/2c450d60-6573-4545-8b20-f6190a87827e" width="126" height="126"/>

</div>

</br>

<div align="center">

[![Discord](https://img.shields.io/badge/Join%20Discord-Collector?color=0363ff&logo=discord&logoColor=white&style=for-the-badge)](https://discord.gg/eV9DF6sBsz)
[![Buy Me a Coffee](https://img.shields.io/badge/Buy%20Me%20a%20Coffee-Ko--fi?color=29abe0&logo=ko-fi&logoColor=white&style=for-the-badge)](https://ko-fi.com/t4ctor)
[![Releases](https://img.shields.io/github/downloads/Tectors/j0.dev/total?style=for-the-badge&color=00ffa2&label=Downloads)](https://github.com/Tectors/j0.dev/releases)

</div>

# 📖 About
**`j0.dev`** aims to elevate the workflow for **JsonAsAsset**, making it easier to set up builds and port assets.

*At the moment it is being used purely to host an API*, but our future goals are to expand this application into a full fledged archive explorer and implementations for JsonAsAsset. (like right clicking a folder to bulk import through JsonAsAsset)

<a name="how-to-use"></a>
# ⭐ How to Use

1. **Visit the Releases Page:**  
   Go to the [Releases page](/../../releases), and download the latest release.
2. **Setup the Application:**  
   Follow the setup process.
3. **Start a new Profile:**  
   Go to the Profiles tab (bottom left) and load a profile, or create one.

# 🧾 Licensing

This project is licensed under the [Apache 2.0 License](./Licensing/Apache-2.0.txt).

**`j0.dev`** makes use of open-source libraries created by many talented developers.
For detailed license information and third-party attributions, please refer to:

* The [Licensing directory](./Licensing)
* The [THIRD\_PARTY\_NOTICES.md](./Licensing/THIRD_PARTY_NOTICES.md) file

To simplify distribution, all license texts have been consolidated into a single [NOTICE](./Licensing/NOTICE) file.

### Clone
```
git clone https://github.com/Tectors/j0.dev --recursive
```

## Build
```
dotnet publish Source/vj0 -c Release --no-self-contained -r win-x64 -o "./Release" -p:PublishSingleFile=true -p:DebugType=None -p:DebugSymbols=false -p:IncludeNativeLibrariesForSelfExtract=true
```

### Update Libraries

```
Dependencies\update_libraries.bat
```

### Update Lisence After Changes

```
python .github/Licensing/main.py
```

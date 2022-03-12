<p align="center">
    <img src="assets/Ost.png" margin="auto"/>
</p>

<div align="center">
    <a href="https://twitter.com/intent/follow?screen_name=blockmountainio">
        <img src="https://img.shields.io/twitter/follow/blockmountainio?style=flat-square&logo=twitter"
            alt="Follow on Twitter"></a>
</div>

<div style="text-align:center">

# Introduction

<p>

OST (Oaks' Solana Toolbox) is a GUI implementation of some tools we've used along the time and that would be a waste to leave gathering dust on the SSD memory chips.
In order to reduce the bootstrapping required this is a direct fork of [Anvil](github.com/bmresearch/anvil).

Expecte more tools, as this will be used to test development some of Project Citadel functionalities.

</p>

<p>

DISCLAIMER: some of these tools won't work in all RPCs as they are very request intensive.

</p>

</div>

## Features

- NFTs
    - Fetching metadata and the NFT
    - Fetching collection mints
    - Fetching current collection owners
    - Export functions
- Full Featured Name Service client
    - .sol <-> address
    - twitter handle <-> address
    - token mints <-> info (ticker, etc)
    - global search by address owner
- Forensic tools
    - Identify top senders and receivers of native token by tx count and volume


## Build

To build OST you will need to have the [.NET Runtime 6.0.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) installed.

To simply run without bothering about publishing, run the following command:

```
dotnet run -c Release --project ./Ost/Ost.csproj
```

To publish the app:

#### Windows

```
dotnet publish -c Release -o ./publish --runtime win-x64 --self-contained true
```

#### macOS x64

```
dotnet publish -c Release -o ./publish --runtime osx.12-x64 --self-contained true
```

#### macOS arm64

```
dotnet publish -c Release -o ./publish --runtime osx.12-arm64 --self-contained true
```

#### Linux x64

```
dotnet publish -c Release -o ./publish --runtime linux-x64 --self-contained true
```

If you wish to publish the application for another runtime, check out the list of available [runtime identifiers](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog#using-rids).


## Support

Consider supporting us:

* Sol Address: **oaksGKfwkFZwCniyCF35ZVxHDPexQ3keXNTiLa7RCSp**
* [Mango Ref Link](https://trade.mango.markets/?ref=MangoSharp)


## Contribution

If you have any other solana tool using C# feel free to contribute and add your own views and functionality.

## Maintainers

* **Hugo** - [murlokito](https://github.com/murlokito)
* **Tiago** - [tiago](https://github.com/tiago18c)

See also the list of [contributors](https://github.com/bmresearch/Solnet/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/bmresearch/Solnet/blob/master/LICENSE) file for details
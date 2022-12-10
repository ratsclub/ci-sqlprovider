{
  description = ".NET project template";

  inputs = {
    nixpkgs.url = "github:nixos/nixpkgs/nixos-unstable";
    utils.url = "github:numtide/flake-utils";
  };

  outputs = inputs@{ nixpkgs, ... }:
    inputs.utils.lib.eachDefaultSystem (system:
      let
        pkgs = import nixpkgs { inherit system; config.allowUnfree = true; };
      in
      rec {
        # `nix develop`
        devShells.default = with pkgs;
          let dotnet-sdk = dotnet-sdk_6;
          in
          mkShell {
            buildInputs = [
              dotnet-sdk
              icu
	      jetbrains.rider
              podman
              podman-compose
            ];

            shellHook = ''
              export DOTNET_ROOT=${dotnet-sdk}
              export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:${lib.makeLibraryPath [ icu  ]}
            '';
          };
      });
}

{
description = "hypr-wellbeing - An app usage logger for hyprland and niri";

inputs.nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";

outputs = { self, nixpkgs }:
  let
    systems = [ "x86_64-linux" ];
    forAllSystems = f: nixpkgs.lib.genAttrs systems (system: f system);
  in
  {
    packages = forAllSystems (system:
      let
        pkgs = nixpkgs.legacyPackages.${system};

        version = "0.0.9";

        binaryName = {
          "x86_64-linux"  = "hypr-wellbeing";
        }.${system};

        sha256 = {
          "x86_64-linux" = "sha256-sem/XST8hSfyFGbAhnLXxFtkhp7uo9k91mL/sEIVBlM=";
        }.${system};

      in
      {
        hypr-wellbeing = pkgs.stdenvNoCC.mkDerivation {
          pname = "hypr-wellbeing";
          inherit version;

          src = pkgs.fetchurl {
            url = "https://github.com/DemonKingSwarn/hypr-wellbeing/releases/download/${version}/${binaryName}";
            inherit sha256;
          };

          dontUnpack = true;
          dontBuild = true;

          nativeBuildInputs = [ pkgs.makeWrapper ];

          propagatedBuildInputs = with pkgs; [ ];

          installPhase = ''
            install -Dm755 $src $out/bin/hypr-wellbeing
          '';

          meta = with pkgs.lib; {
            description = "An app usage logger for hyprland and niri";
            homepage    = "https://github.com/DemonKingSwarn/hypr-wellbeing";
            license     = licenses.gpl3Only;
            maintainers = [ ];
            platforms   = [ "x86_64-linux" ];
            mainProgram = "hypr-wellbeing";
          };
        };

        default = self.packages.${system}.hypr-wellbeing;
      }
    );

    apps = forAllSystems (system: {
      default = {
        type    = "app";
        program = "${self.packages.${system}.hypr-wellbeing}/bin/hypr-wellbeing";
      };
    });
  };
}

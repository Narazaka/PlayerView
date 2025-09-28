# Player View (Sync)

プレイヤーの視点を共有する

## Install

### VCC用インストーラーunitypackageによる方法（おすすめ）

https://github.com/Narazaka/PlayerView/releases/latest から `net.narazaka.vrchat.player-view-installer.zip` をダウンロードして解凍し、対象のプロジェクトにインポートする。

### VCCによる方法

1. https://vpm.narazaka.net/ から「Add to VCC」ボタンを押してリポジトリをVCCにインストールします。
2. VCCでSettings→Packages→Installed Repositoriesの一覧中で「Narazaka VPM Listing」にチェックが付いていることを確認します。
3. アバタープロジェクトの「Manage Project」から「Player View (Sync)」をインストールします。

## Usage

1. 処理のメインである `SyncPlayerView` プレハブをシーンに配置する
2. 画面である `PlayerDisplay` プレハブをシーンに配置する
3. `SyncPlayerView/SyncSelector` または `SyncPlayerView/LocalSelector` のどちらか使う方をActiveにし、使わない方をEditorOnlyにする
4. `SyncPlayerView/SyncSelector` または `SyncPlayerView/LocalSelector` の `_SetTargetPlayerId(int playerId)` を呼ぶようにUdonを設定する
  - たとえば [Player Select UI](https://github.com/Narazaka/PlayerSelectUI) の `Receiver` に `SyncPlayerView/SyncSelector` を設定すると動きます。

## Changelog

- v2.0.1: 依存関係バージョン修正
- v2.0.0: 設計変更
- v1.0.0: リリース

## License

[Zlib License](LICENSE.txt)

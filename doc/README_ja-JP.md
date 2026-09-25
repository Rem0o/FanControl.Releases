<h1 align="center">
  <a href="https://www.getfancontrol.com"><img src="/Images/logo.gif" width="36"/></a><span>&nbsp;</span><span>Fan Control</span>
</h1>

<p align="center">Windows 向けの高度なカスタマイズと機能特化型なファンコントロールソフトウェアです。<br><i>本ソフトウェアのソースコードは非公開です。</i></p>

<p align="center">
  <a href="/FanControl.zip?raw=true"><img src="https://img.shields.io/badge/Download-FanControl-green.svg?style=flat&logo=download" alt="Download"/></a>
  <a href="https://www.paypal.com/donate/?business=N4JPSTUQHRJM8&no_recurring=0&item_name=Fan%20Control%20software%20creator%20and%20maintainer.%20Donations%20allow%20me%20to%20continue%20working%20on%20this%20project%20while%20keeping%20it%20free%20to%20use.%20Thank%20you%20for%20contributing%21&currency_code=USD"><img src="https://img.shields.io/badge/Donate-PayPal-blue.svg?style=flat&logo=paypal" alt="Donate"/></a>
  <a href="https://buy.stripe.com/aFaeV75oSg0wcvhbKL0ZW00"><img src="https://img.shields.io/badge/Donate-Stripe-635BFF?logo=stripe&logoColor=white" alt="Donate"/></a>

</p>

![Fan Control](/Images/MainUI.png)

**README の言語:** [English](/README.md) | **日本語**

---

## 掲載メディア

**JayzTwoCents** — Everyone NEEDS this FREE piece of software... You will thank me!

<a href="https://www.youtube.com/watch?v=uDPKVKBMQU8">
  <img alt="JayzTwoCents - Everyone NEEDS this FREE piece of software... You will thank me!" src="https://i.ytimg.com/vi/uDPKVKBMQU8/hq720.jpg?sqp=-oaymwEcCNAFEJQDSFXyq4qpAw4IARUAAIhCGAFwAcABBg==&rs=AOn4CLDpjcuKgjSlSO8bZt8bcG4eKoRB4Q" width="350"/>
</a>

## 目次

- [お知らせ](#お知らせ)
- [主な機能](#主な機能)
- [インストールとアンインストール](#インストールとアンインストール)
- [プラグイン](#プラグイン)
- [問題とハードウェアの互換性](#問題とハードウェアの互換性)
- [よくある質問](#よくある質問)
- [使用しているライブラリ](#使用しているライブラリ)
- [GitHub Sponsors](#github-sponsors)

## お知らせ

- __NEW__: Fan Control をユーザーセッションなしで、サービスとして起動を行えるようにしました。
- __NEW__: キャリブレーションの回避ポイント: 特定の回転数 (%) でガタつきや不快な異音が発生する場合、その数値を回避ポイントとして設定してください。

  <img alt="JayzTwoCents - Everyone NEEDS this FREE piece of software... You will thank me!" src="/Images/Avoid.png" width="350"/>
- FanControl、インストーラーおよび更新が署名付きの実行ファイルになりました。これによってウイルス対策ソフトによる誤検知が軽減するはずです。
- [V238](https://github.com/Rem0o/FanControl.Releases/releases/tag/V238) 以降から [LHM](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) の [PawnIO](https://pawnio.eu/) が同梱されるようになりました。WinRing0 を FanControl に同梱することをやめたことでウイルス対策ソフトでの問題が解消されます。なお、2025 年 9 月 4 日時点で、V237 以前に使用されていた、WinRing0 (FanControl.sys) は Windows Defender で [Trojan:Win32/Vigorf.A](https://github.com/Rem0o/FanControl.Releases/issues/3410#issuecomment-3254057373) として検知されるようになっています。
- FaceIT の問題は、[PawnIO バージョン 2.1.0](https://github.com/namazso/PawnIO.Setup/releases/tag/2.1.0) で修正されています。

## 主な機能

- 初回起動時のガイド付き**セットアップ**プロセス
- 複数の**プロファイル**を保存、編集、読み込み
- アプリの**テーマ**と**カラー**を変更
- 複数の温度ソース (CPU、GPU、マザーボード、ハードドライブなど...)
- **[カスタムグラフ](#graph-fan-curve-editor)** を含む、複数の **[ファンカーブ機能](https://getfancontrol.com/docs)**
- ファンカーブやセンサーを**ミックス** (最大、最小、平均)
- 低リソースな使用量
- ステップ、開始する %、停止する %、応答時間、ヒステリシスを用いた高度なチューニング機能

詳細なドキュメントは **[getfancontrol.com/docs](https://getfancontrol.com/docs/)** をご確認ください。

## インストールとアンインストール

### インストール

1. [最新のアーカイブをダウンロード](/FanControl.zip?raw=true) *または* [リリースページからインストーラーをダウンロード](https://github.com/Rem0o/FanControl.Releases/releases)してください。
2. 任意のインストール先フォルダーに展開する**または**インストーラーを実行してください。
3. `FanControl.exe` を実行します。

<details>
<summary>その他のインストール方法 (Scoop / Winget / Chocolatey)</summary>

**[Scoop](https://scoop.sh/#/apps?s=2&d=1&o=true&p=1&q=fan+control)**
```
scoop bucket add extras
scoop install fancontrol
```

**[Winget](https://apps.microsoft.com/detail/9nblggh4nns1?rtc=1&hl=en-us&gl=US#activetab=pivot:overviewtab)**
```
winget install Rem0o.FanControl
```

**[Chocolatey](https://community.chocolatey.org/packages/fancontrol/)**
```
choco install fancontrol
```

</details>

### アンインストール

**ポータブル** — ファイルは将来使用するためにそのまま残すことも、削除もできます。

> **注意:** Fan Control を Windows の起動時に開始する設定をしている場合は、Fan Control 内のチェックボックスのチェックを外すか、Windows のタスクスケジューラーで「Fan Control」のタスクを手動で削除してください。

**インストーラー** — 他の Windows プログラムと同様にプログラムの一覧からアンインストールしてください。

## プラグイン

プラグインシステムを使用すると、あらゆる種類のセンサーを Fan Control に組み込むことができます。詳細については [Plugins wiki](https://github.com/Rem0o/FanControl.Releases/wiki/Plugins) をご覧ください。

![プラグインのインストール](/Images/PluginInstallation.png)

### Rem0o の開発

| プラグイン | 説明 |
|--------|-------------|
| [FanControl.IntelCtlLibrary](https://github.com/Rem0o/FanControl.IntelCtlLibrary) | Intel ARC GPU の対応を追加 |
| [FanControl.HWInfo](https://github.com/Rem0o/FanControl.HWInfo) | HWInfo センサーデータをインポート |
| [FanControl.DellPlugin](https://github.com/Rem0o/FanControl.DellPlugin) | Dell 製 ノートとタワーの対応を追加 |

### コミュニティの開発

> *漏れがあった場合はお知らせください。*

| プラグイン | 説明 |
|--------|-------------|
| [FanControl.Thermaltake](https://github.com/AMoo-Miki/FanControlThermaltake) | Thermaltake 製デバイス ([fu-raz/FanControlThermaltake](https://github.com/fu-raz/FanControlThermaltake) の更新版フォーク) |
| [FanControl.LiquidCtl](https://github.com/antoine-bouteiller/FanControl.LiquidCtl) | [liquidctl](https://github.com/liquidctl/liquidctl) 経由の AIO デバイス、マルチファンコントローラーに対応した更新済みフォーク |
| [FanControl.AsusWMI](https://github.com/Mourdraug/FanControl.AsusWMI) | WMI 経由の ASUS マザーボード |
| [FanControl.AquacomputerDevices](https://github.com/medevil84/FanControl.AquacomputerDevices) | Aquacomputer HighFlowNext、Quadro と Octo |
| [FanControl.AquacomputerQuadro](https://github.com/FoPzl/FanControl.AquacomputerQuadro) | Aquacomputer Quadro |
| [FanControl.GPU-Z](https://github.com/vision57/FanControl.GPU-Z) | GPU-Z センサーデータ |
| [FanControl.CorsairLink](https://github.com/EvanMulawski/FanControl.CorsairLink) | Corsair Commander コントローラーと Hydro liquid cooler |
| [FanControl.Razer](https://github.com/EvanMulawski/FanControl.Razer) | Razer 製のデバイス |
| [FanControl.HomeAssistant](https://github.com/hgross/FanControl.HomeAssistant) | [HomeAssistant](https://github.com/home-assistant) 温度センサー (Philips Hue、 HomeMatic、HomeKit など) |
| [FanControl.NzxtKraken](https://github.com/brokenmass/Fancontrol.NzxtKraken) |LHM に含まれていない NZXT Kraken AIO の追加 (例: Kraken X2、X3 new PID) |
| [FanControl.LianLi](https://github.com/lewisgibson/FanControl.LianLi) | LianLi [L-Connect 3](https://lian-li.com/l-connect3/) ファンコントローラー |
| [FanControl.NvThermalSensors](https://github.com/TimSirmovics/FanControl.NvThermalSensors) | Nvidia GPU のホットスポットおよびメモリジャンクションの温度 |
| [FanControl.OpenFan](https://github.com/SasaKaranovic/FanControl.OpenFan) | [OpenFAN](https://github.com/SasaKaranovic/OpenFanController) コントローラー |
| [FanControl.AIDA64](https://github.com/Brian-E-Taylor/FanControl.AIDA64) | AIDA64 センサーの読み取り |
| [FanControl.RazerCoolingPadPlugin](https://github.com/Benson5650/FanControl.RazerCoolingPadPlugin) | Razer Laptop Cooling Pad |
| [FanControl.GPDPlugin](https://github.com/chenx-dust/FanControl.GPDPlugin) | GPD 製デバイス |
| [FanControl.LenovoPlugin](https://github.com/jiarandiana0307/FanControl.LenovoPlugin) | `Lenovo ACPI-Compliant Virtual Power Controller` を搭載した Lenovo 製ノート PC |
| [FanControl.GigabyteWaterforce](https://github.com/brenoperucchi/FanControl.GigabyteWaterforce) | GIGABYTE AORUS WATERFORCE X AIO クーラー — X240、X280、X360 |
| [FanControl.AcerPredatorPH315](https://github.com/phaax/FanControl.AcerPredatorPH315) | Acer Predator Helios 300 (PH315-53) のネイティブファンコントロール |

## 問題とハードウェアの互換性

Fan Control は、既存のハードウェアライブラリの上に構築された UI レイヤーです。ハードウェアの互換性の問題は、主に上流のプロジェクトに依存します:

- **[LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor)** — 主要なセンサー/ドライバーのバックエンド

このリポジトリで Issue を作成するのは、UI のバグや機能のリクエストなど**ソフトウェア自体に関するもののみ**とします。ハードウェアの対応に関するリクエストは、この項目へのリンクを添えて Close させていただきます。

ハードウェアの互換性に関するリクエストがあり、かつ .NET で使用可能な (例: [プラグイン](https://github.com/Rem0o/FanControl.Releases/wiki/Plugins)) **動作する**コードサンプルを提供できる場合は、ぜひ送ってください。

## よくある質問

**Q: FanControl に最適な BIOS 設定はなんですか？**
> BIOS の「スマート」なファン制御機能は使用しないでください。既定の回転数を固定値 (例: 50%) に設定するのが、多くのユーザーにとって最適です。また、BIOS の設定が PWM モードか DC モードかを確認しましょう。構成によっては、どちらか一方がより適している場合があります。

**Q: ノートパソコンでも使えますか？**
> 基本的には無理です。ノートパソコンのファンは、デスクトップのマザーボードとは異なる仕組みで制御されているためです。その多くは、サードパーティー製のソフトウェアから制御できるように設計されていません。ただし、特定のノートパソコンに対応したプラグインが提供されていれば、使用できる可能性があります。念のために補足すると、ファンが認識されなかったり、エラーが発生するかもしれません。

**Q: NVIDIA 製 GPU にはファンが 3 つ搭載されていますが、コントロール用のカードは 2 つしか表示されません。なぜですか？**
> 使用しているカードは 2 つのチャンネルしかなく、複数のファンが同じチャンネルに接続されています。

**Q: NVIDIA GPU のファン回転数が 30% を下回らず、0 RPM になりません。なぜですか？**
> この [Wiki ページ](https://github.com/Rem0o/FanControl.Releases/wiki/Nvidia-30%25-and-0-RPM)をご覧ください。

**Q: コントロールカードがないまたは、コントロールカードがファン速度を変更しません。何か問題があるのでしょうか？**
> [問題点とハードウェアの互換性](#問題とハードウェアの互換性)を参照してください。

**Q: 「ファンカーブの種類」はどのように機能し、そのパラメーターはどのような役割を果たすのでしょうか？**
> 左上にあるカードのアイコンをクリックしてください。説明がダイアログで表示されます。

**Q: どの OS で動作しますか？**
> Windows 10 と Windows 11 です。

## 使用しているライブラリ

| ライブラリ | 目的 |
|---------|---------|
| [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) | メインセンサーソース |
| [MaterialDesignInXamlToolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit) | UI フレームワーク |
| [NvAPIWrapper](https://github.com/falahati/NvAPIWrapper) | Nvidia GPU ファンコントロールとセンサーの読み取り |
| [ADLXWrapper](https://github.com/Rem0o/ADLXWrapper) | AMD GPU ファンコントロールとセンサーの読み取り |
| [gong-wpf-dragdrop](https://github.com/punker76/gong-wpf-dragdrop) | 項目のドラッグアンドドロップ |

## GitHub Sponsors

このページにある GitHub のスポンサーボタンは、**FanControl に関連するオープンソース活動を支援するため**のものです。これには [LibreHardwareMonitor](https://github.com/LibreHardwareMonitor/LibreHardwareMonitor) への貢献、プラグインシステムとオープンソースプラグインの開発、そして [ADLXWrapper](https://github.com/Rem0o/ADLXWrapper) を通じた AMD GPU ドライバーへの対応などが含まれます。

皆様からのスポンサー支援は、既存プロジェクトの維持や新規プロジェクトの開発に役立てられます。なお、この支援は FanControl のメインプログラムのものへの寄付には**該当しません**。FanControl を直接支援したい場合は、[PayPal](https://www.paypal.com/donate/?cmd=_donations&business=N4JPSTUQHRJM8&currency_code=USD&source=url&item_name=Fan+Control) のボタンをご利用ください。

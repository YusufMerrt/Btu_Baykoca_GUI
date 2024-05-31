# BTU_Saturn_GUI Projesi

## Genel Bakış

`BTU_Saturn_GUI` projesi, C# dilinde geliştirilmiş bir Windows Forms uygulamasıdır. Bu GUI uygulaması, bir roket ve onun yükünden gelen telemetri verilerini yönetmek ve görüntülemek için tasarlanmıştır. Uygulama, seri portları yapılandırma, cihazlara bağlanma, telemetri verilerini alma ve işleme ve bu verileri çeşitli grafiksel öğeler üzerinde görüntüleme özelliklerini içerir.

## Özellikler

- **Seri Port Yapılandırması**: Uygulama, kullanıcıya roket ve yük için seri portları yapılandırma imkanı sunar.
- **Telemetri Verisi Alımı**: Uygulama, seri iletişim yoluyla telemetri verilerini alır.
- **Veri İşleme**: Alınan telemetri verileri işlenir ve gerçek zamanlı olarak görüntülenir.
- **Grafiksel Gösterim**: Telemetri verileri çeşitli grafikler üzerinde, örneğin yükseklik, hız, ivme, sıcaklık ve yön (XZ ve XY düzlemleri) grafiklerinde gösterilir.
- **Koşullu Derleme**: Uygulama, ön işlemci direktifleri aracılığıyla roket ve yük özelliklerinin bağımsız olarak etkinleştirilmesini veya devre dışı bırakılmasını sağlayacak şekilde yapılandırılabilir.

## Dosya Yapısı

- **FormGUI.cs**: GUI'nin ve telemetri verilerini işleme mantığının uygulamasını içeren ana form.
- **FormGUI.Designer.cs**: Formu ve kontrollerini oluşturmak için Windows Forms tasarımcısı tarafından üretilen kodu içerir.
- **Program.cs**: Uygulamanın giriş noktası.
- **App.config**: Uygulama ayarlarını ve yapılandırmasını ayarlamak için kullanılan yapılandırma dosyası.

## Kod İncelemesi

### Ön İşlemci Direktifleri

Ön işlemci direktifleri `#define PAYLOAD_ON` ve `#define ROCKET_ON`, yük ve roket özelliklerinin etkin olup olmadığını kontrol eder.

```csharp
#define ROCKET_ON
// #define PAYLOAD_ON
```

### Formun Başlatılması

Form, seri portları başlatır, çeşitli kontroller için varsayılan değerleri ayarlar ve veri alımı için olay işleyicilerini atar.

```csharp
public FormGUI()
{
    InitializeComponent();
}

private void FormGUI_Load(object sender, EventArgs e)
{
    // Roket ve yük için yapılandırma kodu
}
```

### Seri Port Yapılandırması

Form, mevcut seri portları alır ve portları ve baud hızlarını seçmek için combo box'ları doldurur.

```csharp
string[] ports = SerialPort.GetPortNames();
```

### Veri Alımı ve İşleme

Uygulama, seri portlardan veri alımı için olay işleyicilere sahiptir. Alınan veriler işlenir ve gerçek zamanlı olarak görüntülenir.

```csharp
private void RocketDataReceived(object sender, SerialDataReceivedEventArgs e)
{
    // Roket için veri alımını işlemek için kod
}

private void PayloadDataReceived(object sender, SerialDataReceivedEventArgs e)
{
    // Yük için veri alımını işlemek için kod
}
```

### Grafik Güncelleme

Telemetri verileri çözülür ve grafikler üzerinde çizilir.

```csharp
private void updateGraphRocket(object s, EventArgs e)
{
    // Roket veri grafiğini güncellemek için kod
}

private void updateGraphPayload(object s, EventArgs e)
{
    // Yük veri grafiğini güncellemek için kod
}
```

### Yardımcı Fonksiyonlar

Bayt dizilerinden telemetri verilerini çözmek için fonksiyonlar sağlanır.

```csharp
private int decodeR(int bitSize)
{
    // Roket telemetri verilerini çözmek için kod
}

private int decodeP(int bitSize)
{
    // Yük telemetri verilerini çözmek için kod
}
```

## Kullanım

1. **Seri Portları Yapılandırma**: Uygun seri portları ve baud hızlarını seçin.
2. **Cihazlara Bağlanma**: "Bağlan" düğmesine tıklayarak seri portları açın ve telemetri verilerini almaya başlayın.
3. **Verileri Görüntüleme**: Telemetri verileri formdaki metin kutularında ve grafiklerde görüntülenecektir.
4. **Özellikleri Değiştirme**: Zaman güncelleme, yük bırakma ve sistemi sıfırlama gibi özellikleri etkinleştirmek veya devre dışı bırakmak için onay kutularını kullanın.

## Bağımlılıklar

- .NET Framework
- Windows Forms

## Katkıda Bulunma

Bu projeye katkıda bulunmak için, depoyu fork edin ve değişikliklerin ayrıntılı açıklamalarıyla pull request gönderin. Kodunuzun mevcut kod stiline uygun olduğundan ve uygun yorumlar içerdiğinden emin olun.

## Lisans

Bu proje MIT Lisansı altında lisanslanmıştır. Daha fazla bilgi için `LICENSE` dosyasına bakın.

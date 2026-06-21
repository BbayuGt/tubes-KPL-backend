# WebhookController

**Lokasi:** `Controllers/WebhookController.cs`

## Tujuan
Modul ini bertindak sebagai penerima pesan otomatis dari pihak penyedia pembayaran (Xendit). Modul ini bekerja di balik layar tanpa interaksi langsung dari pengguna.

## Cara Kerja
Saat seorang pengguna selesai melakukan transfer atau pembayaran di luar sistem kita (misalnya melalui ATM atau dompet digital), kita perlu tahu bahwa pembayaran itu sudah berhasil.
1. Xendit akan mengirimkan sebuah pesan "Webhook" (pemberitahuan otomatis) ke modul ini.
2. Modul ini pertama-tama akan memeriksa keamanan pesan tersebut, memastikan bahwa pesan itu benar-benar datang dari Xendit dan bukan pihak yang tidak bertanggung jawab.
3. Setelah dipastikan aman, modul ini membaca nomor tagihan dan melihat status terbarunya (misalnya menjadi "SUDAH DIBAYAR" atau *PAID*).
4. Modul ini kemudian memperbarui catatan pembayaran di dalam basis data kita sehingga status donasi pengguna berubah menjadi berhasil.

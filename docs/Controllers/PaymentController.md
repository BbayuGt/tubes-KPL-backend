# PaymentController

**Lokasi:** `Controllers/PaymentController.cs`

## Tujuan
Modul ini adalah jembatan yang menghubungkan aplikasi donasi dengan penyedia layanan pembayaran pihak ketiga (dalam hal ini, Xendit). Modul ini bertugas membuat tagihan pembayaran secara elektronik.

## Cara Kerja
Proses yang terjadi saat pengguna siap untuk membayar adalah:
1. Aplikasi mengirimkan permintaan ke modul ini berisi informasi donasi (seperti total uang dan tujuan kampanye).
2. Modul ini akan memastikan nominal donasi tersebut benar (lebih dari nol).
3. Modul ini secara aman menggunakan kunci rahasia untuk menghubungi server Xendit.
4. Sistem Xendit akan membuatkan "Faktur Pembayaran" (Invoice) yang berisi tautan (URL) untuk pengguna membayar.
5. Setelah berhasil, modul ini menyimpan catatan pembayaran tersebut ke dalam sistem dengan status "Tertunda" (*Pending*), lalu mengirimkan tautan pembayaran tersebut kembali ke pengguna agar mereka dapat segera mentransfer dana.

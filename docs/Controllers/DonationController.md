# DonationController

**Lokasi:** `Controllers/DonationController.cs`

## Tujuan
Modul ini berfungsi untuk menerima, mencatat, dan menyediakan riwayat donasi yang dilakukan oleh pengguna ke berbagai kampanye.

## Cara Kerja
Fungsi-fungsi yang dilayani oleh modul ini meliputi:
1. **Melihat Riwayat Seluruh Donasi:** Modul ini dapat memberikan laporan berisi daftar semua donasi yang pernah terjadi di dalam sistem.
2. **Melihat Detail Satu Donasi:** Mengambil data spesifik dari sebuah transaksi donasi berdasarkan nomor identitas transaksinya (ID).
3. **Mencatat Donasi Baru:** Ini adalah fungsi utama di mana pengguna mengirimkan dana. Modul ini menerima data donasi yang masuk, melakukan pengecekan awal (misalnya memastikan nominal tidak kurang dari nol), lalu memerintahkan sistem untuk menyimpannya.

Pekerjaan perhitungan, validasi aturan bisnis, dan penyimpanan diserahkan kepada **DonationService**. Jika ada kesalahan pada saat pengguna memasukkan data, modul ini akan menolaknya dan memberikan pesan kesalahan yang mudah dipahami.

# AppDbContext

**Lokasi:** `Data/AppDbContext.cs`

## Tujuan
Modul ini adalah jantung dari pengelolaan data aplikasi. Ini adalah penghubung (jembatan) resmi antara kode program yang kita tulis dengan mesin basis data fisik (PostgreSQL).

## Cara Kerja
Aplikasi kita menggunakan teknologi yang disebut *Entity Framework Core* untuk berinteraksi dengan basis data tanpa perlu menulis perintah SQL secara manual. `AppDbContext` bertugas sebagai koordinator utama untuk hal ini.

1. **Mendaftarkan Tabel:** Modul ini mendefinisikan tabel-tabel apa saja yang harus ada di dalam basis data, seperti tabel Pengguna (Users), Kampanye (Campaigns), Donasi (Donations), dan Pembayaran (Payments).
2. **Melakukan Perubahan:** Setiap kali ada layanan (Service) yang ingin menambah, mengubah, atau menghapus data, mereka akan meminta tolong kepada `AppDbContext` untuk menyimpannya ke dalam PostgreSQL.

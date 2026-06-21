# GenericRepository

**Lokasi:** `Repositories/GenericRepository.cs` & `Repositories/IGenericRepository.cs`

## Tujuan
Modul ini bertindak sebagai lapisan abstraksi antara aplikasi dan basis data (`AppDbContext`). Dengan menggunakan pola Repositori Generik, kita memisahkan logika akses data dari logika bisnis di layanan (Services).

## Cara Kerja
Daripada setiap layanan berbicara langsung dengan `AppDbContext`, mereka akan menggunakan Repositori ini.
1. **Dapat Digunakan Berulang:** Repositori ini bersifat "Generik" (`<T>`), artinya satu kode repositori ini dapat digunakan untuk berbagai entitas tabel (seperti `User`, `Campaign`, `Donation`, dan `Payment`) tanpa perlu menulis ulang logika dasar.
2. **Operasi Standar (CRUD):** Menyediakan metode-metode standar untuk menambah (Add), membaca (Get), memperbarui (Update), dan menghapus (Delete) data.
3. **Penyimpanan Terpusat:** Repositori ini memastikan bahwa perubahan-perubahan data dimonitor dan hanya disimpan ke PostgreSQL (melalui `SaveChangesAsync`) ketika semua operasi selesai, menjaga keamanan dan integritas data.
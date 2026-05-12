# AuthService

**Lokasi:** `Services/AuthService.cs`

## Tujuan
Modul ini berisi aturan dan logika khusus terkait dengan identitas pengguna. Ini memisahkan logika keamanan dari bagian penerimaan permintaan (Controller).

## Cara Kerja
Layanan ini bekerja di balik layar untuk membantu Controller mengenali siapa pengguna yang sedang mengakses sistem.
1. Layanan ini membaca informasi unik (klaim identitas) dari pengguna yang sedang masuk.
2. Berdasarkan informasi tersebut, layanan ini akan mencari profil lengkap pengguna di dalam basis data (melalui AppDbContext).
3. Jika pengguna ditemukan, data profil akan dikembalikan agar dapat ditampilkan oleh sistem.

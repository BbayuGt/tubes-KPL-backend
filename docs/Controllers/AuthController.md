# AuthController

**Lokasi:** `Controllers/AuthController.cs`

## Tujuan
Modul ini bertugas sebagai pintu gerbang untuk urusan identitas pengguna. Setiap kali pengguna ingin mengakses informasi profil pribadi mereka, aplikasi akan melewati modul ini.

## Cara Kerja
Modul ini memastikan bahwa hanya pengguna yang memiliki izin (sudah masuk atau *login*) yang dapat mengakses profil mereka. 
1. Saat aplikasi meminta data pengguna saat ini, modul ini memeriksa kunci keamanan (token) yang dikirim.
2. Jika kuncinya sah, modul ini akan mengambil data pengguna dari sistem.
3. Untuk menjaga keamanan, informasi rahasia seperti kata sandi tidak akan dikembalikan. Modul ini menyaring data dan hanya mengirimkan informasi yang aman seperti Nama, Email, dan ID Pengguna.

Modul ini terhubung langsung dengan bagian **AuthService** untuk menjalankan proses validasi identitas tersebut.

# Dokumentasi Backend Aplikasi Donasi

Proyek ini merupakan sistem backend untuk aplikasi donasi, yang dibangun menggunakan arsitektur modern berbasis ASP.NET Core, PostgreSQL, dan terintegrasi dengan Xendit sebagai gerbang pembayaran. Sistem ini mengatur kampanye donasi, transaksi, dan autentikasi pengguna secara aman.

## Panduan Mulai Cepat (Quickstart)

Sistem ini wajib dijalankan menggunakan **Docker**. Pendekatan ini memastikan lingkungan yang konsisten dan mempermudah instalasi.

1. **Persiapan:** Pastikan Docker dan Docker Compose sudah terpasang dan berjalan di komputer Anda.
2. **Menjalankan Proyek:** Buka terminal, arahkan ke folder utama proyek (di mana file `compose.yaml` berada), dan jalankan perintah:
   ```bash
   docker compose up --build
   ```
3. **Proses:** Docker akan secara otomatis mengunduh komponen yang diperlukan, menyiapkan basis data PostgreSQL, dan menjalankan aplikasi.
4. **Selesai:** Setelah proses selesai, aplikasi backend dapat diakses melalui `http://localhost:8080`. Basis data juga otomatis terhubung dan siap digunakan.

## Penjelasan Modul

Untuk memudahkan pemahaman sistem, dokumentasi proyek dibagi berdasarkan modul-modul berikut. Klik pada tautan untuk membaca penjelasan lebih rinci:

### Pengendali (Controllers)
Bagian ini adalah pintu masuk utama untuk semua permintaan dari aplikasi klien (seperti website atau aplikasi seluler).
*   [AuthController](docs/Controllers/AuthController.md) - Mengatur proses masuk dan identifikasi pengguna.
*   [CampaignController](docs/Controllers/CampaignController.md) - Mengelola daftar dan detail kampanye donasi.
*   [DonationController](docs/Controllers/DonationController.md) - Mengelola pencatatan donasi dari pengguna.
*   [PaymentController](docs/Controllers/PaymentController.md) - Menghubungkan aplikasi dengan sistem pembayaran untuk membuat tagihan.
*   [WebhookController](docs/Controllers/WebhookController.md) - Menerima pembaruan status pembayaran secara otomatis dari gerbang pembayaran.

### Layanan Bisnis (Services)
Bagian ini memproses aturan bisnis dan logika utama dari aplikasi.
*   [AuthService](docs/Services/AuthService.md) - Menangani logika validasi akun pengguna.
*   [CampaignService](docs/Services/CampaignService.md) - Menangani logika pengambilan dan penambahan kampanye donasi.
*   [DonationService](docs/Services/DonationService.md) - Menangani aturan perhitungan dan pencatatan transaksi donasi.

### Basis Data & Struktur
Bagian ini mengatur bagaimana data disimpan dan dihubungkan.
*   [AppDbContext](docs/Data/AppDbContext.md) - Mengatur komunikasi antara kode aplikasi dengan mesin basis data PostgreSQL.
*   [Models (Model Data)](docs/Models/Models.md) - Penjelasan struktur tabel yang digunakan (seperti data pengguna, donasi, dan pembayaran).

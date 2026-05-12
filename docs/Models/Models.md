# Struktur Data (Models)

**Lokasi:** Direktori `Models/`

## Tujuan
Bagian ini mendefinisikan bentuk, sifat, dan struktur dari setiap informasi yang disimpan oleh aplikasi. Ini seperti cetak biru dari lemari arsip kita.

## Daftar Model Utama

Berikut adalah penjelasan sederhana dari tipe data yang kita kelola:

### 1. User (Pengguna)
Mewakili orang yang menggunakan sistem.
*   **Informasi yang disimpan:** ID unik, Nama Lengkap, Alamat Email, dan Kata Sandi (yang disimpan secara rahasia dan aman).

### 2. Campaign (Kampanye Donasi)
Mewakili sebuah program penggalangan dana.
*   **Informasi yang disimpan:** Judul kampanye, Deskripsi singkat, Target dana yang ingin dicapai, dan Total dana yang sudah terkumpul sejauh ini.

### 3. Donation (Donasi)
Mewakili satu transaksi sumbangan dari pengguna untuk suatu kampanye.
*   **Informasi yang disimpan:** Jumlah uang yang didonasikan, Waktu terjadinya donasi, serta catatan kampanye mana yang menerima dana tersebut.

### 4. Payment (Pembayaran)
Mewakili catatan tagihan yang dibuat melalui sistem pihak ketiga (Xendit).
*   **Informasi yang disimpan:** Nomor identitas eksternal (dari Xendit), Status pembayaran (seperti *PENDING* atau *PAID*), Tautan untuk membayar (URL Invoice), serta jumlah tagihan.

### 5. XenditSettings
Mewakili pengaturan internal aplikasi untuk dapat terhubung dengan server Xendit.
*   **Informasi yang disimpan:** Kunci rahasia (Secret Key) dan Token otorisasi untuk keamanan komunikasi.

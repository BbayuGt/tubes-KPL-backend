# CampaignService

**Lokasi:** `Services/CampaignService.cs`

## Tujuan
Modul ini mengelola seluruh aturan bisnis yang berkaitan dengan kampanye penggalangan dana. Layanan ini menjadi perantara antara Controller (yang meminta data) dan AppDbContext (yang menyimpan data).

## Cara Kerja
Fungsi utama dari layanan ini adalah memproses data kampanye:
1. **Mengambil Data:** Layanan ini dapat menarik seluruh daftar kampanye dari basis data, atau mencari satu kampanye tertentu berdasarkan identitasnya (ID).
2. **Menyimpan Data Baru:** Saat ada permintaan pembuatan kampanye baru, layanan ini akan memastikan datanya lengkap dan kemudian memerintahkan basis data untuk menyimpannya dengan aman.

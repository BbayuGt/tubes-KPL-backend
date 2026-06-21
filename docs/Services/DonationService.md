# DonationService

**Lokasi:** `Services/DonationService.cs`

## Tujuan
Modul ini menangani logika utama di balik proses pencatatan setiap dana yang masuk dari donatur. Layanan ini memastikan setiap keping donasi tercatat dengan benar pada kampanye yang tepat.

## Cara Kerja
1. **Menarik Riwayat:** Dapat mencari dan mengembalikan daftar donasi, baik secara keseluruhan maupun satu per satu berdasarkan ID.
2. **Memproses Donasi Baru:** Ini adalah tugas terpentingnya. Saat seseorang menyumbang, layanan ini akan:
   - Mencatat detail donasi tersebut ke dalam sistem.
   - Menambahkan jumlah uang yang didonasikan ke total uang yang terkumpul pada kampanye tersebut (memperbarui saldo kampanye).
3. Melalui layanan ini, kita memastikan bahwa catatan transaksi dan laporan saldo kampanye selalu seimbang dan akurat.

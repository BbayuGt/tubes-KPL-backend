# WebhookService

**Lokasi:** `Services/WebhookService.cs`

## Tujuan
Modul ini bertugas menangani pemrosesan dari beban muatan (payload) webhook yang diterima oleh sistem secara lebih luas, mengatur logika tambahan yang berjalan sesuai dengan kejadian (event) yang diterima.

## Cara Kerja
1. **Pemetaan Kejadian (Event Mapping):** Layanan ini menggunakan kamus data (Dictionary) untuk memetakan jenis event (seperti `donation.success`, `donation.failed`, atau `campaign.closed`) kepada fungsi penangan (handler) yang spesifik.
2. **Pemrosesan Terarah:** Saat webhook diterima, layanan ini akan mencari fungsi penangan berdasarkan tipe event dan langsung mengeksekusi operasi terkait tanpa perlu struktur kendali percabangan ganda (if-else) yang kompleks.
3. **Pembaruan Sistem:** Untuk event seperti donasi berhasil, layanan ini berinteraksi dengan `AppDbContext` untuk memperbarui jumlah donasi yang terkumpul (`CollectedAmount`) pada kampanye terkait secara sinkron.

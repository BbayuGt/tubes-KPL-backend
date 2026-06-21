# CampaignController

**Lokasi:** `Controllers/CampaignController.cs`

## Tujuan
Modul ini bertanggung jawab untuk melayani semua informasi yang berkaitan dengan kampanye donasi (program penggalangan dana). 

## Cara Kerja
Modul ini menyediakan beberapa jalur komunikasi untuk aplikasi klien:
1. **Menampilkan Semua Kampanye:** Klien dapat meminta daftar seluruh kampanye donasi yang sedang aktif atau tersedia di sistem.
2. **Melihat Detail Kampanye:** Jika pengguna ingin melihat satu kampanye secara spesifik (misalnya untuk membaca deskripsi atau melihat target donasi), modul ini akan mencarikan kampanye tersebut berdasarkan nomor identitasnya (ID).
3. **Membuat Kampanye Baru:** Modul ini juga melayani permintaan untuk menambahkan program penggalangan dana baru ke dalam sistem.

Seluruh perintah ini diteruskan ke **CampaignService** yang bertugas melakukan pekerjaan utama mencari atau menyimpan data di basis data.

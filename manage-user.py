import sys
import subprocess

def main():
    if len(sys.argv) != 3:
        print("Usage: python manage-user.py <userId> <RoleBaru>")
        sys.exit(1)

    user_id = sys.argv[1]
    new_role = sys.argv[2]

    # Validasi bahwa user_id adalah angka
    if not user_id.isdigit():
        print("Error: <userId> harus berupa angka.")
        sys.exit(1)

    # Menghindari issue kutip tunggal dalam SQL
    safe_new_role = new_role.replace("'", "''")

    # Query SQL untuk melakukan update
    sql_query = f'UPDATE "Users" SET "Role" = \'{safe_new_role}\' WHERE "Id" = {user_id};'

    # Perintah docker compose exec
    command = [
        "docker", "compose", "exec", "-T", "database",
        "psql", "-U", "postgres", "-d", "tubes_kpl_db", "-c", sql_query
    ]

    try:
        # Menjalankan perintah shell
        result = subprocess.run(command, capture_output=True, text=True, check=True)
        output = result.stdout.strip()
        
        # psql mengembalikan teks seperti "UPDATE 1" jika sukses merubah 1 baris
        if output == "UPDATE 1":
            print(f"Sukses! Role untuk user ID {user_id} berhasil diubah menjadi '{new_role}'.")
        elif output == "UPDATE 0":
            print(f"Error: User dengan ID '{user_id}' tidak ditemukan.")
        else:
            print(f"Output: {output}")
            
    except subprocess.CalledProcessError as e:
        print("Terjadi kesalahan saat mengeksekusi perintah Docker:")
        print(e.stderr.strip() or e.stdout.strip())
        sys.exit(1)
    except FileNotFoundError:
        print("Error: Perintah 'docker' tidak ditemukan. Pastikan Docker sudah terinstall dan berjalan.")
        sys.exit(1)

if __name__ == "__main__":
    main()

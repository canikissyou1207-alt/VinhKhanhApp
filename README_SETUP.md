# Vinh Khanh Street Guide - bản nâng cấp

## Có gì mới
- Web admin để quản lý POI tại `/AdminPois`
- Upload ảnh và audio trực tiếp từ web admin
- API `/api/POIs` trả ảnh/audio với URL đầy đủ
- App MAUI đọc dữ liệu mới, dùng bán kính từ DB, hỗ trợ mở audio URL nếu có và fallback về TTS
- Tự tạo database và seed mẫu khi API chạy lần đầu

## Cách chạy nhanh

### 1) SQL Server
Sửa connection string trong `VinhKhanhApi/appsettings.json` nếu cần. Mặc định đang dùng:

```json
"DefaultConnection": "Server=.;Database=VinhKhanhDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

### 2) Chạy API
Mở `VinhKhanhApi` bằng Visual Studio và chạy project.

Sau khi chạy xong:
- Swagger: `http://localhost:5174/swagger`
- Admin: `http://localhost:5174/AdminPois`
- API JSON: `http://localhost:5174/api/POIs`

### 3) Chạy app MAUI
Mở `VinhKhanhApp` rồi chạy Android emulator.
App mặc định gọi:
- Android: `http://10.0.2.2:5174`
- Windows: `http://localhost:5174`

## Ghi chú
- Audio upload đã có trong admin và API. Trên app, nếu có `AudioUrl`, app sẽ thử mở audio URL trước; nếu không được thì dùng Text-to-Speech.
- Vì môi trường hiện tại không có .NET SDK, mã nguồn đã được nâng cấp theo cấu trúc đúng nhưng chưa build thử trong container này.

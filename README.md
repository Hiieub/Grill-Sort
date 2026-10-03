<div align="center">
<img src="https://capsule-render.vercel.app/api?type=waving&height=180&color=gradient&customColorList=12,20,24&text=Grill%20Sort&fontSize=56&fontColor=ffffff&fontAlignY=38&desc=&descSize=18&descAlignY=60" alt="Grill Sort" width="100%">

<a href="https://hiieu.itch.io/grill-sort">
  <img src="https://img.itch.zone/aW1nLzMwNDg3Mjc5LnBuZw==/315x250%23c/uKUj5e.png" alt="Ảnh bìa Grill Sort" width="180">
</a>

<br><br>

[![Play on itch.io](https://img.shields.io/badge/▶%20Chơi%20ngay-itch.io-fa5c5c?logo=itch.io&logoColor=white&style=for-the-badge)](https://hiieu.itch.io/grill-sort)
[![Developer](https://img.shields.io/badge/Dev-Hiieu-ff7a1a?logo=itch.io&logoColor=white&style=for-the-badge)](https://hiieu.itch.io)
[![Platform](https://img.shields.io/badge/Nền%20tảng-Web%20%2F%20itch.io-2a2520?style=for-the-badge)](https://hiieu.itch.io/grill-sort)

<!-- </div>
---
<div align="center"> -->

</div>


## Giới thiệu
- Đây là một project cá nhân được phát triển nhằm mục đích học tập và rèn luyện kỹ năng.
- Gameplay được clone dựa trên thể loại Sorting Puzzle.
## Công nghệ & Kỹ thuật sử dụng
- **Engine:** Unity 6000.3
- **Ngôn ngữ:** C#
- **Kiến trúc / Design Pattern:** 
  - Singleton Pattern
  - State Machine 
  - Observer Pattern (Event-driven)
- **Kỹ thuật:**
  - **Pre-allocation & Reuse:** Tái sử dụng object (tránh Instantiate/Destroy liên tục) để tối ưu memory.
  - **Data Serialization (JSON):** Parsing dữ liệu màn chơi động từ file JSON (`JsonUtility`).
  - **Thuật toán / Logic:** Thuật toán xáo trộn và phân bổ item ngẫu nhiên có điều kiện; Logic tự động nhận diện & merge item.
  - **UI/UX:** Hệ thống UI Panel độc lập; Sử dụng DOTween để xử lý UI/Gameplay animation.

<img src="https://capsule-render.vercel.app/api?type=waving&height=100&color=gradient&customColorList=12,20,24&section=footer" width="100%" alt="">

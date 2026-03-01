# ByeVS-Memo
무거운 Visual Studio 환경을 벗어나, VS Code와 .NET SDK만으로 가볍게 C# WPF의 기본 구조와 이벤트 처리 흐름을 이해하기 위해 제작한 첫 번째 윈도우 데스크톱 토이 프로젝트임.

## 🛠️ 기술 스택 및 환경 (Tech Stack & Tools)
- **Language**: C#
- **Framework**: .NET 8.0, WPF (Windows Presentation Foundation)
- **IDE / Editor**: Visual Studio Code (C# Dev Kit)
- **Tools**: .NET CLI, Git, GitHub

## 📝 업데이트 내역 (Changelog)

### [2026-03-01]
* **다크모드 지원**: UI 테마(다크/라이트 모드) 전환 기능 추가
- **WPF 화면 구성 (XAML)**: `Grid`와 `StackPanel`을 활용하여 상단 버튼 영역(열기/저장)과 메인 텍스트 입력 영역(`TextBox`)을 분리하여 직관적인 UI를 설계함.
- **파일 입출력(I/O) 연동**: `OpenFileDialog`와 `SaveFileDialog`를 통해 사용자가 내 컴퓨터의 `.txt` 파일을 직접 선택해 불러오고, 작성한 내용을 텍스트 파일로 저장할 수 있도록 C# 이벤트 핸들러 로직을 구현함.
- **경량화 빌드 환경 구축**: 무거운 IDE 없이 터미널의 `dotnet new wpf` 명령어와 VS Code만으로 프로젝트를 생성하고, `dotnet run`으로 즉각적인 빌드 및 실행이 가능하도록 환경을 세팅함.
- **Git 버전 관리 최적화**: `.NET` 전용 `.gitignore`를 적용하여 `bin/`, `obj/` 등 불필요한 임시 빌드 결과물을 배제하고, 순수 소스 코드만 깃허브 원격 저장소에 업로드되도록 저장소 상태를 깔끔하게 정립함. 
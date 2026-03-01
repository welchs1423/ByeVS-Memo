# 🚀 ByeVS-Memo

무거운 Visual Studio 없이, VS Code와 .NET SDK만으로 가볍게 개발한 C# WPF 메모장 애플리케이션입니다.

## ✨ 핵심 기능
* **파일 열기**: 내 컴퓨터의 `.txt` 파일을 불러옵니다.
* **파일 저장**: 작성한 내용을 새로운 텍스트 파일로 저장합니다.
* **가벼운 실행**: 복잡한 IDE 종속성 없이 `dotnet run` 명령어로 즉시 실행 가능합니다.

## 🛠️ 기술 스택
* **Language**: C#
* **Framework**: .NET 8.0 (WPF)
* **IDE**: Visual Studio Code

## 🚀 시작하는 법
1. 저장소를 클론합니다.
2. 터미널(VS Code)에서 프로젝트 폴더로 이동합니다.
3. 아래 명령어를 실행합니다.
   ```bash
   dotnet run

---

## 🐙 Git에 올리고 스마트하게 관리하기

이제 터미널에 아래 명령어들을 순서대로 입력해서 깃에 코드를 저장해 볼 건데요. 

코드를 원격 저장소에 푸시(Push)할 때마다 **`README.md` 파일이 수정되었는지 터미널에 콕 집어서 알려주는 똑똑한 알림 기능**도 명령어에 함께 세팅해 두었습니다. 관리하기 훨씬 편하실 거예요!

```bash
# 1. 깃 초기화
git init

# 2. 푸시할 때 README 수정 여부를 알려주는 스마트 알림(Git Hook) 설정
mkdir -p .git/hooks
echo '#!/bin/sh' > .git/hooks/pre-push
echo 'if git diff HEAD~1 HEAD --name-only | grep -q "README.md"; then' >> .git/hooks/pre-push
echo '  echo "🔔 [알림] 이번 푸시에 README.md 파일 수정 내역이 포함되어 있습니다!"' >> .git/hooks/pre-push
echo 'fi' >> .git/hooks/pre-push

# 3. 모든 파일 추가 및 첫 커밋
git add .
git commit -m "🎉 첫 커밋: ByeVS-Memo 기본 기능 구현 및 README 추가"
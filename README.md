# Suika-Game

<img src="https://github.com/user-attachments/assets/bd980423-7e0a-4785-9cf1-01413a327098" width="187"></img>
<img src="https://github.com/user-attachments/assets/7e51723b-128b-4694-838f-2b4be5265b2a" width="187"></img>
<img src="https://github.com/user-attachments/assets/df63a9a3-53cc-4205-b21a-7cccb259787f" width="187"></img>
<img src="https://github.com/user-attachments/assets/f69dcdcd-9bb6-47bd-a89a-dc3780b2a384" width="187"></img>
</br>

## 1. 게임 메커니즘
  - 위에서 과일이 생성되며, 드래그하여 특정위치에 내려 놓아 같은 종류의 과일끼리 부딫히면 합쳐져 더 큰 과일로 성장
  - 과일의 크기는 순서대로 커지며, 최종적으로 "수박"까지 합치며 고득점을 얻는 게임
## 2. 주요 목표
  - 주어진 제한된 공간에서 가능한 한 많은 과일을 합쳐 높은 점수를 기록
## 3. 개발 환경
  - Unity, C#

</br>

## 개발 일지
[2023-10-31]
- Project Setting
- Add Circle
  - Add Circle Prefab
  - Add Level당 Circle Animations
  - Add 같은 Level의 Circle끼리 합쳐지면 LevelUp System
  - Add Effect Particle System
    - Circle이 합쳐질때 Effect 효과
- Add GameOver System
</br>

[2023-11-01]
- Add Object Pooling
  - Circle Object Pooling System 제작
- 코드 정리
</br>

[2023-11-02]
- Add UI & Sprite
</br>

[2023-11-03]
- Add Circle DropLine
  - 떨어지는 라인 생성
    - Object Pooling으로 인해 이미 비활성화된 DropLine을 다시 활성화 하는 코드 작성 해야함
</br>

[2023-11-07]
- Fix Circle DropLine
  - DropLine 비활성화 이슈 해결
- Add GameScene & TitleScene
  - 기존 Scene을 분리함
- Fix GameManager
  - Invoke코드 삭제
- Change Mobile Build Setting (모바일 빌드로 세팅)
- Del Font
</br>

[2023-11-08]
- Add NextCircle & UI
  - 다음에 보여질 Circle 제작
- Fix TouchScreenSize
</br>

[2023-11-09]
- Fix DropLine
</br>

[2023-11-13]
- Set WebGL
  - WebGL에 올려보기
</br>

[2023-11-27]
- Update README.md
</br>

[2023-11-30]
- 코드 정리
</br>

[2023-12-01]
- Update Project Setting
</br>

[2024-11-22]
- Update TItleScene UI
- Build Test
- 코드 정리
- 1차 제작 완료
</br>



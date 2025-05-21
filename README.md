![ChatGPT Image 2025년 5월 21일 오전 07_58_25](https://github.com/user-attachments/assets/78780fee-a54d-4b52-90dc-0bca75f68ba7)

## 📘 목차
- [프로젝트 소개](#프로젝트-소개)
- [개발 기간](#개발-기간)
- [사용 기술](#사용-기술)
- [핵심 로직](#핵심-로직)

<br/>

---

## 🔍 프로젝트 소개
**ImpossibleBosses**는 **Unity 6** 기반으로 개발된 **멀티플레이 PvE 보스 레이드 게임**입니다.  
플레이어는 최대 **8명**까지 팀을 이루어 거대한 보스 몬스터에 도전하며,  
**아처(Archer)**, **파이터(Fighter)**, **메이지(Mage)**, **몽크(Monk)**, **네크로맨서(Necromancer)** 중  
총 **5개 클래스** 중 하나를 선택할 수 있습니다.

보스의 다양한 공격 패턴을 팀원 간 **협동과 전략**으로 공략하며,  
전투를 통해 **아이템을 획득**하고, **장비를 교체**하면서  
플레이어는 점차 강해져 **최종 보스를 처치하는 것**이 목표입니다.

<br/>

---

## 📆 개발 기간
🗓 **2024-12-11 ~ (진행 중)**

<br/>

---

## 🔧 사용 기술
| 구분 | 스택 |
| :-- | :-- |
| **Engine** | `Unity 6` |
| **Networking** | `Unity Netcode` |
| **Voice & Chat** | `Vivox Service` |
| **Auth / DB** | `Google OAuth 2`, `Google Spreadsheet` |
| **AI Behavior** | `Behaviour Tree Designer` |
| **Test Tool** | `Unity Play Mode Scenarios` |

<br/>

---

## 🗝 핵심 로직

### 🔐 로그인
1. **`LoginScene`** 로드  
2. 플레이어가 **로그인 / 회원가입** 선택  
3. `LogInManager`가 **Google 스프레드시트**(UserAuthenticateData)와 ID·PW 대조  
4. **인증 성공 → 로비 씬**으로 이동  

![Login](https://github.com/user-attachments/assets/acc72412-c500-49c4-8d64-7f26c8e4a62e)

---

### 📥 데이터 불러오기
- 로그인 직후 **`DataManager`** 가 Google 스프레드시트에서 **플레이어 정보** **몬스터 정보** **아이템 목록**을 비동기로 불러와
  로드된 데이터는 **`Dictionary`** 로 캐싱하여 이후 씬에서도 재사용  

---

### 🏠 로비
1. **계정 인증** 후 **`LobbyScene`** 전환  
2. `LobbyManager`가 Unity Services **초기화 → 익명 인증 → WaitLobby 접속**  
3. 플레이어는 글로벌 채팅으로 소통하고 방 **생성 / 참가** 가능  

---

### 🔗 릴레이 서버
- **호스트(방장)** 가 방 생성 → **Relay Allocation** 수신(서버 역할)  
- **참가자** 는 **Join Code** 로 해당 Allocation 접속  
- 실제 IP 노출 없이 **NAT / 방화벽** 문제 해결  
- 호스트가 중도 이탈하면 **새 호스트가 Allocation 승계**

![Relay](https://github.com/user-attachments/assets/24e4cb53-1ba9-4500-9ddb-59168fc3628e)

---

### 🏎️ 최적화 방법
| 영역 | 왜 이런 방법을 썼는가? |
| :-- | :-- |
| **오브젝트 풀링 &<br/>네트워크 오브젝트 풀링** | 스킬·파티클 등 **자주 생성·삭제** 오브젝트를 미리 생성해 **재사용** → GC 최소화 |
| **네트워크 패킷 절감** | **필요 데이터만 압축 전송** → 이후 계산은 **로컬**에서 수행 -> 네트워크 비용 최소화 |

---

### 🌐 네트워크 동기화
1. **서버 권한 구조**  
   - 보스 AI·게임 판정을 **호스트**가 전담  
   - 결과(피해량·상태) → **NetworkVariable / RPC** 로 전송  
2. **예측 & 보간**  
   - 입력은 **로컬 예측** → 서버와 차이 발생 시 부드럽게 보정  
   - 원격 객체 위치는 **보간**으로 끊김 제거  

---

![Boss Flow](https://github.com/user-attachments/assets/33e41408-493a-4778-830d-c0c69d4055a5)

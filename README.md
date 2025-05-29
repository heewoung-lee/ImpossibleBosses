![ChatGPT Image 2025년 5월 21일 오전 07_58_25](https://github.com/user-attachments/assets/78780fee-a54d-4b52-90dc-0bca75f68ba7)

## 📘 목차
- **[프로젝트 소개](#-프로젝트-소개)**
- **[개발 기간](#-개발-기간)**
- **[사용 기술](#-사용-기술)**
- **[핵심 로직](#-핵심-로직)**

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
🗓 **2024-12-11 ~ **

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

![Image](https://github.com/user-attachments/assets/a63eec10-7526-4920-bd92-319d0a640e82)

---

### 📥 데이터 불러오기
ImpossibleBosses의 데이터 관리는 Managers.DataManager를 중심으로 이루어집니다. 이 매니저는 Google 스프레드시트에서 게임에 필요한 각종 데이터(플레이어 정보, 몬스터 정보, 아이템 목록 등)를 로드하고, 애플리케이션 내에서 효율적으로 사용할 수 있도록 처리합니다.

<p align="center">
  <strong>&lt;데이터 시트&gt;</strong>
</p>
<p align="center">
  <img src="https://github.com/user-attachments/assets/5fa4ab70-ba04-403b-b229-c403439998e1" alt="데이터 시트 이미지 1" width="70%"/>
  <img src="https://github.com/user-attachments/assets/66638094-07c2-48ad-b48e-744c3f8d9183" alt="데이터 시트 이미지 2" width="70%"/>
</p>
<p align="center">
  <img src="https://github.com/user-attachments/assets/8c104aa5-92eb-44ea-82e5-c77787290c39" alt="데이터 시트 이미지 3" width="70%"/>
</p>


**데이터 로딩 절차:**

1.  **초기화 및 타입 스캔**:
    * `Managers.DataManager.Init()` 메서드가 데이터 로딩을 시작합니다.
    * `LoadSerializableTypesFromFolder` 메서드는 지정된 경로에서 `[Serializable]` 어트리뷰트를 가진 클래스들을 리플렉션으로 스캔합니다. 이 클래스들은 스프레드시트의 각 시트 데이터 구조와 매핑됩니다.
  <p align="center">
  <img src="https://github.com/user-attachments/assets/f0fcfdb3-cd07-494a-9edd-267df547bfd1" alt="타입 스캔 이미지 1" width="70%"/>
</p>

2.  **Google 스프레드시트 연동**:
    * `DatabaseStruct`는 Google OAuth 2.0 인증 정보(클라이언트 ID, 시크릿 코드, 애플리케이션 이름, 스프레드시트 ID)를 관리합니다.
    * `GetGoogleSheetData()` 메서드는 이 정보를 사용하여 Google Sheets API 인증 후, 지정된 스프레드시트 데이터를 가져옵니다.
    <p align="center">
  <img src="https://github.com/user-attachments/assets/47be08dd-43b7-4740-9853-89e74ab992f3" alt="스프레드시트 연동" width="70%"/>
</p>

3.  **데이터 파싱 및 구조화**:
    * `LoadDataFromGoogleSheets()`는 인증된 서비스와 스프레드시트 ID로 각 시트의 데이터를 요청합니다.
    * `ParseSheetData()`는 시트 데이터를 JSON 형식 문자열로 변환합니다.
    * `AddAllDataDictFromJsonData()`는 JSON 문자열을 C# 객체로 역직렬화합니다.
        * `GetTypeNameFromFileName()`은 시트 이름에서 데이터 타입을 결정합니다.
        * `FindGenericKeyType()`은 데이터 타입이 `Ikey<TKey>` 인터페이스를 구현했는지 확인하여 딕셔너리 키 타입을 결정합니다.
        * `DataToDictionary<TKey, TStat>` 클래스는 로드된 데이터 리스트를 `Dictionary<TKey, TStat>` 형태로 변환하여 `AllDataDict`에 저장합니다.

4.  **데이터 캐싱 및 접근**:
    * 처리된 데이터는 `DataManager.AllDataDict` (`Dictionary<Type, object>` 타입)에 데이터 타입별로 캐싱되어, 게임 내 다른 시스템에서 사용됩니다.
    * `ItemDataManager`는 `DataManager.AllDataDict`에서 아이템 관련 타입의 데이터를 가져와 관리합니다.

5.  **로컬 데이터 활용**:
    * Google 스프레드시트 접근 불가 시, `LoadAllDataFromLocal()` 메서드가 로컬에 JSON 파일로 저장된 데이터를 로드합니다.
    * 스프레드시트에서 새 데이터를 가져오면, `SaveDataToFile()` 메서드가 기존 로컬 데이터와 비교 후 변경된 경우 최신 데이터로 덮어씁니다. `BinaryCheck<T>()`가 데이터 변경 여부를 확인합니다.

---

### 🏠 로비
1. **계정 인증** 후 **`LobbyScene`** 전환  
2. `LobbyManager`가 Unity Services **초기화 → 익명 인증 → 로비 접속**  
3. 플레이어는 글로벌 채팅으로 소통하고 방 **생성 / 참가** 가능  

---

### 🔗 릴레이 서버
- **호스트(방장)** 가 방 생성 → **Relay Allocation** 수신(서버 역할)  
- **참가자** 는 **Join Code** 로 해당 Allocation 접속  
- 실제 IP 노출 없이 **NAT / 방화벽** 문제 해결  
- 호스트가 중도 이탈하면 **새 호스트가 Allocation 승계**

![Image](https://github.com/user-attachments/assets/4d7df461-dbe3-4e5d-aeb1-72d5b26841ff)

---

### 🏎️ 최적화 방법
| 영역 | 왜 이런 방법을 썼는가? |
| :-- | :-- |
| **오브젝트 풀링 &<br/>네트워크 오브젝트 풀링** | 스킬·파티클 등 **자주 생성·삭제** 오브젝트를 미리 생성해 **재사용** → GC 최소화 |
| **네트워크 패킷 절감** | **필요 데이터만 압축 전송** → 이후 계산은 **로컬**에서 수행 -> 전송량 절감, 반응속도 상승 |

---

### 🌐 네트워크 동기화
1. **서버 권한 구조**  
   - 보스 AI·게임 판정을 **호스트**가 전담  
   - 결과(피해량·상태) → **NetworkVariable / RPC** 로 전송  
2. **예측 & 보간**  
   - 패킷을 전송할때 서버가 패킷을 보낸시간과 클라이언트가 패킷을 받은시간을 계산해 서버와 차이 발생 시 부드럽게 보간으로 보정  

---

![Boss Flow](https://github.com/user-attachments/assets/33e41408-493a-4778-830d-c0c69d4055a5)

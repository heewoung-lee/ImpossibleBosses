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
  <img src="https://github.com/user-attachments/assets/8c104aa5-92eb-44ea-82e5-c77787290c39" alt="데이터 시트 이미지 3" width="70%"/>
</p>


**데이터 로딩 절차:**

1.  **초기화 및 타입 스캔**:
    <ul>
      <li><code>Managers.DataManager.Init()</code> 메서드가 데이터 로딩을 시작합니다.</li>
      <li><code>LoadSerializableTypesFromFolder</code> 메서드는 지정된 경로에서 <code>[Serializable]</code> 어트리뷰트를 가진 클래스들을 리플렉션으로 스캔합니다. 이 클래스들은 스프레드시트의 각 시트 데이터 구조와 매핑됩니다.</li>
    </ul>

<p align="center">
  <strong>&lt;DataManger의 타입확인&gt;</strong>
</p>
<div align="center">
  <img src="https://github.com/user-attachments/assets/f0fcfdb3-cd07-494a-9edd-267df547bfd1" alt="타입 스캔 이미지 1" width="70%"/>
</div>
<br>

2.  **Google 스프레드시트 연동**:
    <ul>
      <li><code>DatabaseStruct</code>는 Google OAuth 2.0 인증 정보(클라이언트 ID, 시크릿 코드, 애플리케이션 이름, 스프레드시트 ID)를 관리합니다.</li>
      <li><code>GetGoogleSheetData()</code> 메서드는 이 정보를 사용하여 Google Sheets API 인증 후, 지정된 스프레드시트 데이터를 가져옵니다.</li>
    </ul>

<p align="center">
  <strong>&lt;구글 스프레드시트 불러오기&gt;</strong>
</p>
<div align="center">
  <img src="https://github.com/user-attachments/assets/47be08dd-43b7-4740-9853-89e74ab992f3" alt="스프레드시트 연동" width="70%"/>
</div>
<br>

3.  **데이터 파싱 및 구조화**:
    * `LoadDataFromGoogleSheets()`는 인증된 서비스와 스프레드시트 ID로 각 시트의 데이터를 요청합니다.
    * `ParseSheetData()`는 시트 데이터를 JSON 형식 문자열로 변환합니다.
    * `AddAllDataDictFromJsonData()`는 JSON 문자열을 C# 객체로 역직렬화합니다.
        * `GetTypeNameFromFileName()`은 시트 이름에서 데이터 타입을 결정합니다.
        * `FindGenericKeyType()`은 데이터 타입이 `Ikey<TKey>` 인터페이스를 구현했는지 확인하여 딕셔너리 키 타입을 결정합니다.
        * `DataToDictionary<TKey, TStat>` 클래스는 로드된 데이터 리스트를 `Dictionary<TKey, TStat>` 형태로 변환하여 `AllDataDict`에 저장합니다.

<p align="center">
  <strong>&lt;JSON 문자열 역직렬화&gt;</strong>
</p>
<div align="center">
  <img src="https://github.com/user-attachments/assets/5d878784-0047-4341-a7d5-eaf3cad0e707" alt="데이터 파싱" width="70%"/>
</div>
<br>

4.  **데이터 캐싱 및 접근**:
    * 처리된 데이터는 `DataManager.AllDataDict` (`Dictionary<Type, object>` 타입)에 데이터 타입별로 캐싱되어, 게임 내 다른 시스템에서 사용됩니다.
    * `ItemDataManager`는 `DataManager.AllDataDict`에서 아이템 관련 타입의 데이터를 가져와 관리합니다.
<p align="center">
  <strong>&lt;아이템 클래스의 데이터 캐싱&gt;</strong>
</p>
<div align="center">
  <img src="https://github.com/user-attachments/assets/3a36e9ed-a833-4e30-b9ac-41880a50a860" alt="데이터 캐싱" width="70%"/>
</div>
<br>


5.  **로컬 데이터 활용**:
    * Google 스프레드시트 접근 불가 시, `LoadAllDataFromLocal()` 메서드가 로컬에 JSON 파일로 저장된 데이터를 로드합니다.
    * 스프레드시트에서 새 데이터를 가져오면, `SaveDataToFile()` 메서드가 기존 로컬 데이터와 비교 후 변경된 경우 최신 데이터로 덮어씁니다. `BinaryCheck<T>()`가 데이터 변경 여부를 확인합니다.
<p align="center">
  <strong>&lt;데이터 변경 확인(바이너리비교)&gt;</strong>
</p>
<div align="center">
  <img src="https://github.com/user-attachments/assets/56236faf-6a3d-4658-bc5e-fdad2c12e310" alt="데이터 변경확인" width="30%"/>
</div>
<br>

---

### 🏠 로비 (Lobby)
플레이어는 계정 인증 후 로비 화면으로 이동하여, 다른 플레이어들과 소통하고 함께 게임을 즐길 방을 찾거나 만들 수 있습니다.

로비에서의 주요 활동:

로비 입장 및 준비:

로비 화면에 처음 들어오면, 게임은 자동으로 기본적인 연결 설정을 진행합니다. 이 과정에는 플레이어의 익명 인증, 이미 다른 곳에서 접속 중이지 않은지 확인하는 절차 등이 포함됩니다.
모든 준비가 끝나면, 플레이어들이 잠시 머무르며 방을 탐색할 수 있는 '대기 로비'에 자동으로 참가하게 됩니다. 만약 대기 로비가 없다면 새로 만들어집니다.
&lt;p align="center">
&lt;strong>&amp;lt;로비 접속 과정&amp;gt;&lt;/strong>
&lt;/p>
&lt;p align="center">
&lt;img src="[여기에 로비 접속 시퀀스 다이어그램 또는 로딩 화면 이미지 삽입]" alt="로비 접속 과정" width="70%"/>
&lt;br/>
&lt;sub>플레이어가 로비에 접속하고 준비되는 과정을 나타내는 흐름도 또는 로딩 화면&lt;/sub>
&lt;/p>
&lt;br/>

소통과 교류:

로비에서는 다른 플레이어들과 음성 또는 텍스트로 자유롭게 대화할 수 있는 채팅 기능이 제공됩니다.
이 기능을 통해 함께 게임 할 파티원을 구하거나 정보를 교환할 수 있습니다.
&lt;p align="center">
&lt;strong>&amp;lt;로비 채팅 화면&amp;gt;&lt;/strong>
&lt;/p>
&lt;p align="center">
&lt;img src="[여기에 로비 채팅 UI (UI_LobbyChat 또는 UI_RoomChat) 이미지 삽입]" alt="로비 채팅" width="60%"/>
&lt;br/>
&lt;sub>다른 플레이어와 대화할 수 있는 채팅창&lt;/sub>
&lt;/p>
&lt;br/>

게임 방 탐색 및 참가:

현재 만들어져 있는 다른 플레이어들의 게임 방 목록을 실시간으로 확인할 수 있습니다. 목록을 새로고침하여 최신 상태를 볼 수도 있습니다.
마음에 드는 방을 발견하면 '참가' 버튼을 눌러 해당 방으로 들어갈 수 있습니다. 만약 방에 비밀번호가 설정되어 있다면, 비밀번호를 입력해야 합니다.
&lt;p align="center">
&lt;strong>&amp;lt;방 목록 및 방 정보&amp;gt;&lt;/strong>
&lt;/p>
&lt;p align="center">
&lt;img src="[여기에 방 목록 UI (UI_Room_Inventory) 및 방 정보 패널 (UI_Room_Info_Panel) 이미지 삽입]" alt="방 목록" width="70%"/>
&lt;br/>
&lt;sub>생성된 게임 방들의 목록과 각 방의 상세 정보 (방 이름, 인원 수 등)&lt;/sub>
&lt;/p>
&lt;br/>

게임 방 생성:

직접 게임 방을 만들 수도 있습니다. 방 만들기 화면에서 방의 이름, 최대 참가 인원, 그리고 필요하다면 비밀번호를 설정할 수 있습니다.
이렇게 생성된 방은 다른 플레이어들의 방 목록에도 나타나게 됩니다.
&lt;p align="center">
&lt;strong>&amp;lt;방 생성 설정&amp;gt;&lt;/strong>
&lt;/p>
&lt;p align="center">
&lt;img src="[여기에 방 생성 UI (UI_CreateRoom) 이미지 삽입]" alt="방 생성" width="50%"/>
&lt;br/>
&lt;sub>새로운 게임 방을 만들기 위한 설정 화면 (방 이름, 인원, 비밀번호 입력)&lt;/sub>
&lt;/p>
&lt;br/>

캐릭터 선택 및 게임 시작:

게임 방에 성공적으로 들어가면, 플레이어는 자신이 플레이할 캐릭터를 선택하는 화면으로 이동합니다.
이곳에서 다른 플레이어들이 어떤 캐릭터를 선택했는지, 준비를 마쳤는지 등을 확인할 수 있습니다. 모든 플레이어가 준비를 마치면 방장이 게임을 시작할 수 있습니다.
&lt;p align="center">
&lt;strong>&amp;lt;캐릭터 선택 화면&amp;gt;&lt;/strong>
&lt;/p>
&lt;p align="center">
&lt;img src="[여기에 캐릭터 선택 UI (UI_Room_CharacterSelect 내의 CharacterSelectorNGO 프레임들) 이미지 삽입]" alt="캐릭터 선택" width="70%"/>
&lt;br/>
&lt;sub>방에 참가한 플레이어들이 각자 플레이할 캐릭터를 고르고 준비하는 모습&lt;/sub>
&lt;/p>
&lt;br/>

로비의 안정적인 운영:

방을 만든 플레이어(호스트)는 로비가 계속 유지될 수 있도록 주기적으로 신호를 보냅니다.
로비 내에서 플레이어가 나가거나 들어오는 등의 변화는 모든 참가자에게 실시간으로 알려져 화면에 반영됩니다.

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

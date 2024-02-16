# DeSummer
### 스마트팩토리 알루미늄 보온로 관리 프로그램
생산품의 품질을 좌우할 온도 유지와, 보온로에서 알루미늄을 추출하기 위한 제어 밸브를 PLC와 C#을 통해 자동 제어함으로 효율적인 생산관리와 안전성, 인건비 등의 많은 이점을 제공하는 프로그램입니다.

#### 소개:
👩‍💻👨‍💻 **Team**
</br>
팀명 : DESUMMER</br>
팀장 : [이지율](https://github.com/JiyulLe)</br>
팀원 : [정주은](https://github.com/gatemap), [김영웅](https://github.com/YwKim98)
</br></br>

**📃주요 기능:**
1. **다양한 보온로 지원:** A, B, C 등 다양한 유형의 보온로를 지원하여 생산 라인의 다양한 요구에 대응합니다.
2. **실시간 데이터 모니터링:** 알루미늄 보온로의 온도, 상태 등을 실시간으로 모니터링하여 생산 프로세스를 쉽게 파악할 수 있습니다. [마이팩토리데이터](https://myfactorydata.com/bbs/board.php?bo_table=data_3&wr_id=1)에서 제공한 실제 데이터를 csv 파일로 기록된 것을 사용했습니. (담당자 : 정주은, 김영웅)
3. **알림 및 경고 시스템:** 고온, 저온 또는 이상 상태일 때 자동으로 알림을 받아 예방 조치를 할 수 있습니다. (담당자 : 이지율)
4. **생산 계획 통합:** 생산 계획을 효과적으로 관리하고 보온로의 작동을 최적화하여 생산성을 향상시킵니다.
5. **사용자 친화적 인터페이스:** 직관적이고 사용자 친화적인 인터페이스를 통해 쉽게 조작하고 모니터링할 수 있습니다.
6. **보온로 자동 제어 :** 저온과 정상온도를 감지하여 생산을 자동 제어할 수 있습니다. (담당자 : 이지율)
7. **알루미늄 생성 밸브 제어:** HMI를 통해서 밸브를 수동/자동으로 제어할 수 있습니다. (담당자 : 김영웅)
8. **제품 불량 검출** : 영상처리 기술을 활용하여 제품의 정상과 불량을 구분할 수 있습니다. (담당자 : 이지율)
9. **로그인** : 다뤄지는 데이터의 보안을 위하여 허가된 인원만 사용할 수 있습니다. DB와 C#을 MySqlConnector 라이브러리를 사용해서 연동하고, 입력받은 ID와 비밀번호를 비교하여 로그인 시스템 구축.(담당자 : 정주은)

</br></br>
**🔧사용 도구🔨:**
|도구 명세서||
|------|---|
|문서작성 도구|velog|
|프로젝트 공유|[github](https://github.com/gatemap/DeSummer)|
|커뮤니케이션 도구|슬랙, 구글 스프레드 시트|


|개발도구|버전(라이브러리)|
|------|---|
|C# WPF|Visual Studio 2022|
|.NET|7.0|
|NuGet|Scottplot(ver 4.1.68),</br> WPF UI(ver 2.1.0)|
|XG5000|4.75 ver|
|XP-Builder|3.80.0605 ver|
|Git||
+ Windows 운영체제 (Windows 10 이상)

**🔌PLC 기기 : LS Electric**
|LS PLC|모델명|
|------|------|
|CPU|XGI|
|통신 모듈|XGL-EFMT(B)|
|입력 모듈|XGI-A21A/C|
|출력 모듈|XGQ-RY1A|

⚠️ HMI와 PLC의 LD 프로그래밍을 확인하기 위해서는 위의 표(개발 도구)에서 언급한 XG5000 프로그램과 XP-Builder 프로그램이 필요합니다.

### How to excute
#### 얼굴 인식
사진(얼굴 정면 사진) 경로를 지정하고 웹캠을 켜서 사진과 대조하는 방식으로 동작. 여러 각도에서 촬여한 데이터가 있다면 좀더 빨리 인식할 수 있음.</br>
**FaceRecognition.cs** 참조. 사진 등록은 사진 불러오는 함수에 직접 이미지 경로를 입력한다.

#### MySQL 세팅
1. [MySql Download](https://dev.mysql.com/downloads/installer/) 해당 링크에서 자신의 컴퓨터와 맞는 OS로 설치를 하고 권장 방식으로 해서 전부 다운로드 받는다.
2. Mysql Workbench를 통해서 최초 서버 계정 등록을 해준다. 참조글 : (https://hongong.hanbit.co.kr/mysql-%EB%8B%A4%EC%9A%B4%EB%A1%9C%EB%93%9C-%EB%B0%8F-%EC%84%A4%EC%B9%98%ED%95%98%EA%B8%B0mysql-community-8-0/)
3. 신규 Table을 생성할 때에, Table 명을 userdata로 한다.
   - 꼭 userdata로 하지 않아도 되지만 그렇게 될 경우, **SqlControl.cs** 파일의 테이블명을 모조리 바꿔야 한다.
   - PK는 userIndex로 설정하며, 데이터 타입은 Int, auto increase 체크를 해준다. userId는 Unique 설정을 해주고, VARCHAR로 설정하고 크기는 최소 30 이상으로 설정한다.
   - userPassword는 VARCHAR로 설정하고 크기는 최소 50 이상으로 설정한다.
   - userName은 VARCHAR로 설정하고, 크기는 최소 20 이상으로 설정한다.
   - admin은 tinyInt로 설정하고, default value를 0으로 설정한다.
   - 위에 언급된 컬럼명들은 다 원하는대로 변경해도 되지만 그런 경우 위에서 언급한 **SqlControl.cs** 파일에서의 컬럼명들을 명시해둔 부분을 모두 바꿔야 한다.
4. **SqlControl.cs**에 언급한대로 누구나 접근할 수 있는 계정 접근에 대해서 설정하거나, 계정 접근에 대해서 SQL문으로 권한을 주는 명령어를 실행한다. MySql 명렁어 참조 : [W3Schools_MySql](https://www.w3schools.com/mysql/mysql_exercises.asp)

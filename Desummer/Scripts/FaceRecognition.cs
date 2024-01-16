using OpenCvSharp;
using FaceRecognitionDotNet;
using NumSharp;


namespace Desummer.Scripts
{
    public class FaceRecognitions
    {
        List<FaceEncoding> knownFaceEncodings; // 알려진 얼굴 인코딩을 저장하는 리스트
        List<string> knownFaceNames; // 알려진 얼굴 이름을 저장하는 리스트 
        FaceRecognition faceRecognition; // FaceRecognition 객체
        public static string face_login { get; set; } // 로그인에 필요한 데이터를 저장하는 속성

        public FaceRecognitions() // 생성자
        {
            knownFaceEncodings = new List<FaceEncoding>(); // 리스트 초기화
            knownFaceNames = new List<string>(); // 리스트 초기화

            faceRecognition = FaceRecognition.Create("D:\\RData\\model_data"); // 학습모델 데이터 폴더주소로 FaceRecognition 객체 생성

            LoadKnownFaces(); // 알려진 얼굴 데이터 로드
        }

        public void LoadKnownFaces() // 알려진 얼굴 이미지 등록 메소드
        {

            // 등록 인물1
            // 얼굴 사진 경로
            var youngImage = FaceRecognition.LoadImageFile("D:\\RData\\face_data\\young.jpg");
            // 얼굴 사진을 Dlib의 FaceRecognition 모델들의 데이터를 사용하여 특징을 추출하여 인코딩
            var youngFaceEncoding = faceRecognition.FaceEncodings(youngImage).FirstOrDefault();
            // 인코딩된 얼굴을 knownFaceEncodings 리스트에 추가
            knownFaceEncodings.Add(youngFaceEncoding);
            // 인물 이름을 knownFaceNames 리스트에 추가
            knownFaceNames.Add("Youngwoong Kim");

            // 등록 인물2
            var jiyulImage = FaceRecognition.LoadImageFile("D:\\RData\\face_data\\jiyul.jpg");
            var jiyulFaceEncoding = faceRecognition.FaceEncodings(jiyulImage).FirstOrDefault();
            knownFaceEncodings.Add(jiyulFaceEncoding);
            knownFaceNames.Add("Jiyul Lee");
        }

        public void StartFaceRecognition() // 실시간 얼굴 인식 메소드
        {
            VideoCapture videoCapture = new VideoCapture(0); // 웹캠에서 영상을 가져오는 객체
            bool processThisFrame = true; // 프레임 처리 여부를 결정하는 플래그
            var startTime = DateTime.Now; // 시작 시간

            while (true) // 무한 루프
            {
                using var frame = new Mat(); // 영상의 프레임을 저장하는 Mat 객체
                
                videoCapture.Read(frame); // 웹캠의 현재 프레임을 읽어서 frame에 저장

                if (processThisFrame) // 프레임을 처리해야 할 경우
                {
                    if ((DateTime.Now - startTime).TotalSeconds > 10) // 10초안에 인식안되면 break
                    {
                        face_login = "";
                        break;
                    }

                    // 프레임의 크기를 줄이고, 색상 공간을 BGR에서 RGB로 변환
                    var smallFrame = frame.Resize(new OpenCvSharp.Size(), 0.5, 0.5);
                    var rgbSmallFrame = new Mat();
                    Cv2.CvtColor(smallFrame, rgbSmallFrame, ColorConversionCodes.BGR2RGB);

                    // RGB 이미지를 파일로 저장
                    // 라이브러리 간의 호환성 문제로 인해 이미지 파일을 생성하여 비교
                    Cv2.ImWrite("temp.png", rgbSmallFrame);

                    // 저장한 이미지를 불러와서 FaceRecognition 이미지 객체로 변환
                    var image = FaceRecognition.LoadImageFile("temp.png");

                    // 이미지에서 얼굴 위치를 검출
                    var faceLocations = faceRecognition.FaceLocations(image).ToList();

                    // 얼굴이 검출되지 않은 경우, 다음 프레임으로 넘어감
                    if (!faceLocations.Any()) 
                    {
                        continue;
                    }

                    // 얼굴 위치에서 얼굴 인코딩을 계산
                    var faceEncodings = faceRecognition.FaceEncodings(image, faceLocations).ToList();

                    // 얼굴 인코딩이 계산되지 않은 경우, 다음 프레임으로 넘어감
                    if (!faceEncodings.Any())
                    {
                        continue;
                    }

                    // 인식된 얼굴 이름을 저장하는 리스트
                    var faceNames = new List<string>();

                    // 얼굴 대조
                    foreach (var faceEncoding in faceEncodings)
                    {
                        // 알려진 얼굴 인코딩과 비교하여 가장 가까운 얼굴을 찾음
                        var matches = FaceRecognition.CompareFaces(knownFaceEncodings.ToArray(), faceEncoding).ToArray();
                        face_login = "Unknown"; // 미등록 인물을 인식하였을 경우 

                        // 알려진 얼굴 인코딩과의 특징간의 거리를 계산
                        var faceDistances = knownFaceEncodings.Select(knownEncoding => FaceRecognition.FaceDistance(knownEncoding, faceEncoding)).ToArray();

                        // 거리 배열을 NumSharp의 NDArray로 변환
                        var nsFaceDistances = new NDArray(faceDistances);

                        // 거리가 가장 작은 얼굴의 인덱스를 찾음
                        var bestMatchIndex = nsFaceDistances.argmin();

                        if (matches[bestMatchIndex]) // 얼굴이 알려진 얼굴인 경우
                        {
                            face_login = knownFaceNames[bestMatchIndex]; // 해당 얼굴의 이름을 face_login에 저장
                        }

                        faceNames.Add(face_login); // 인식된 얼굴 이름을 리스트에 추가
                    }

                    processThisFrame = !processThisFrame; // 다음 프레임을 처리하지 않도록 설정

                    if (Cv2.WaitKey(1) == 'q') // 'q' 키가 눌리면 루프를 탈출
                    {
                        break;
                    }

                    if (faceNames.Count > 0) // 얼굴 이름이 하나 이상 인식된 경우, 루프를 탈출
                    {
                        if(App.userData == null)
                        {
                            App.userData = new UserData(string.Empty, string.Empty, false);
                            App.userData.isFaceLogin = true;
                        }
                        break;
                    }
                }
                if (System.IO.File.Exists("temp.png")) // 인식용으로 사용한 이미지 오작동 방지 삭제
                {
                    System.IO.File.Delete("temp.png");
                }
            }
            videoCapture.Release(); // 릴리즈 비실행시 마지막에 찍혔던 frame이 남게되어 그 frame이 인식됨
            Cv2.DestroyAllWindows();
        }
    }
}
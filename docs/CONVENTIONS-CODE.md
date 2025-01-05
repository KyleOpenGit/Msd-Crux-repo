# 포매팅 및 네이밍 코딩 컨벤션
----
----
# ■ Database 컨벤션
## 네이밍
모든 항목에 `소문자` & `snake_case` 적용
- 데이터베이스 & 스키마 이름
- 테이블이름 & 칼럼 이름
- key 이름 (No SQL)

예시:
- `my_database`.`my_schema`
- `my_table`
- `my_column`
- `"my_key" : "values have no generic conventions"`

## id 칼럼
숫자형 PK 칼럼의 레코드는 `1` 부터 시작 (0이하 없음)


----
----

# ■ C# 코드 컨벤션

## EditorConfig 적용
코드 포매팅 규칙은 다양한 코드 에디터에서 공통으로 적용가능한 [EditorConfig](https://editorconfig.org)를 사용합니다. 이를 위해 `.editorconfig` 파일이 레포지토리에 포함되었습니다.

 > 첨부된 `.editorconfig` 파일의 내용은 공개된 [MS 닷넷팀 editorconfig 파일](https://github.com/dotnet/runtime/blob/main/.editorconfig)의 내용과 동일합니다.

각 개발자들은 자신이 사용하는 코드에디터/IDE에 EditorConfig를 적용하여 코드포매팅을 적용하거나,  [Git Hooks](https://git-scm.com/book/ko/v2/Git맞춤-Git-Hooks)의 `pre-commit` hook을 사용하면 커밋 전 포매팅이 자동으로 적용되도록 할 수 있습니다.


[PRE-COMMIT.md](PRE-COMMIT.md) 내용을 확인해서 git pre-commit을 설정하십시오.


* IDE/코드에디터에서 EditorConfig 적용하기:
  * Visual Stuido: [EditorConfig를 사용하여 일관된 코딩 스타일 정의](https://learn.microsoft.com/ko-kr/visualstudio/ide/create-portable-custom-editor-options?view=vs-2022)
  * Jetbrains의 Rider: Use [EditorConfig](https://www.jetbrains.com/help/rider/Using_EditorConfig.html)
  * VS Code: [EditorConfig for VS Code](https://marketplace.visualstudio.com/items?itemName=EditorConfig.EditorConfig) 공식 확장프로그램


## 컨벤션

### 접근제한자
인터페이스 외에는 private(기본)도 접근제한자를 명시

### 타입선언: var 사용규칙
- Primitive type 변수선언에는 var 사용 안함 (int, string 등)
- 기타 타입에대해 var 사용시엔 변수명을 타입명과 동일한 이름으로 선언
  - 변수명을 타입이름과 다르게 선언하려면 var를 사용하지 않고 반드시 명시적으로 타입 선언


### 대소문자(casing)
- 파스칼 케이스 (PascalCase)
  - 네임스페이스(디렉토리), 클래스(&파일), 열거형, 메소드(함수), Public 변수(프라퍼티), 상수
- 카멜 케이스 (camelCase)
  - private 필드(전역변수), 지역변수, 매개변수

#### 약어 처리 (UI, XML, HTTP 등)
약어가 포함된 경우 약어를 모두 대문자로 쓸지, 단어처럼 혼합형으로 쓸지에 따라 다음과 같은 변형이 가능합니다:
.	파스칼 케이스에서 약어 예:
	•	모두 대문자 유지: UIKit, XMLParser, HTTPRequest
	•	일반 단어처럼 취급: UiKit, XmlParser, HttpRequest

약어에 대해 다음 컨벤션 적용:
- 네임스페이스 명에서는 약어를 모두 대문자로 유지
  - `HTTPHelper`
- 그 외 클래스(파일)등에서는 일반 단어처럼 취급
  - `HttpHelper`

## 접두어 (suffix)

- 일반 private 필드 (전역변수)
  - 언더바 접두어 적용
    - ex) `_myPrivate`

## 매개변수 명
생성자, 메소드에서의 매개변수 이름은 접두어 없는 카멜 케이스 사용.

단, C# v12.0에 소개된 Primary Constructor(프라이머리 생성자, 주요 생성자) 매개변수 이름은 private 필드처럼 언더바(`_`)를 접두어로 붙이도록 합니다.

~~~
public class MyClass(IServiceProvider _provider)  //주생성자에서 이름 _provider
{
    public void DoSomething()
    {
        var service = _provider.GetService(typeof(SomeService)); //호출 코드에서
    }
}
~~~

> 이는 프라이머리 생성자를 C# 12 이전스타일로 변경해야 될 때 호출측 코드를 변경하지 않기 위함입니다.


## 변수에대한 규칙
* 일반 인스턴스 필드는 public 금지: public 으로 노출할 일반필드는 모두 프라퍼티로 변환해 사용하고 프라퍼티 네이밍 규칙에 따름
* 상수필드는 Public 허용, 상수필드 네이밍 규칙 적용

    | 필드 유형             | 접근 제한자         | 접두어 | 네이밍 스타일 | 예                     |
    |-----------------------|--------------------|--------|----------------|------------------------|
    | 상수 필드 (const)     | public, private 등 | 없음   | PascalCase     | MaxItems, Pi          |
    | 정적 필드 (static)    | private 등         | s_     | camelCase      | s_counter, s_logger   |
    | 인스턴스 필드         | private 등         | _      | camelCase      | _userName, _age       |



* 프로퍼티 (public):
앞글자 대문자: 일반 프라퍼티, 정적 프라퍼티
    ~~~
    public int Age { get; set; }
    public static string AppName { get; set; }
    ~~~

* 일반 필드& 정적 필드:
  * 반드시 private. (public 선언해야 할 경우 프로퍼티 사용)
  * 일반 인스턴스 필드(전역 변수) : 앞글자 언더바
  * 정적 필드 : 앞글자 s_ 접두어
    ~~~
    private int _age;
    private static string s_appName;
    ~~~


* 상수:
  * const 또는 readonly 필드: 앞글자 대문자  (public 필드 허용)
    ~~~
    public const double Pi = 3.14159;
    public const string DefaultUserName = "Admin";
    ~~~


그외에는 아래의 MS 닷넷팀 컨벤션 요약을 참고.


##  ■ MS 닷넷팀 editorconfig 파일의  C# 컨벤션 설명

### 줄바꿈 스타일
- 중괄호는 항상 새 줄에서 시작.
- else, catch, finally 등은 새 줄에서 시작.
- 객체 초기화 및 익명 타입 멤버는 각 줄에 작성.
- LINQ 쿼리 각 절은 새 줄에 작성.

~~~
// 중괄호는 항상 새 줄에서 시작
if (condition)
{
    Console.WriteLine("Condition is true");
}
else
{
    Console.WriteLine("Condition is false");
}

// else, catch, finally 등은 새 줄에서 시작
try
{
    // Some code
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    Console.WriteLine("Cleanup code");
}

// 객체 초기화 멤버는 각 줄에 작성
var person = new Person
{
    Name = "John",
    Age = 30,
    Location = "New York"
};

// 익명 타입 멤버는 각 줄에 작성
var anonymous = new
{
    Name = "Jane",
    Age = 25,
    Location = "London"
};

// LINQ 쿼리에서 각 절은 새 줄로 작성
var query = from item in items
            where item.IsActive
            select item.Name;
~~~

### 들여쓰기 스타일
- 블록의 내용은 들여쓰기.
- 중괄호 자체는 들여쓰기하지 않음.
- switch-case의 내용은 들여쓰기, case 레이블은 한 단계 덜 들여쓰기.
~~~
// 블록의 내용은 들여쓰기
if (condition)
{
    Console.WriteLine("Indented Content");
}

// 중괄호 자체는 들여쓰기하지 않음
switch (value)
{
case 1:
    Console.WriteLine("Case 1");
    break;
case 2:
    Console.WriteLine("Case 2");
    break;
default:
    Console.WriteLine("Default Case");
    break;
}
~~~

### 네이밍 컨벤션
### 1. 상수 (const)
- const로 선언된 상수는 PascalCase를 사용.
~~~
// PascalCase 사용
public const double Pi = 3.14159;
public const int MaxItems = 100;
public const string DefaultName = "Admin";
~~~

### 2 정적 필드 (static)
- static 필드는 s_ 접두어와 camelCase를 사용.
~~~
// s_ 접두어와 camelCase 사용
private static int s_counter;
private static string s_logger;
~~~

### 3 내부/비공개 필드 (private/internal)
- private 또는 internal 필드는 _ 접두어와 camelCase를 사용.
~~~
// _ 접두어와 camelCase 사용
private int _age;
internal string _name;
~~~

## 접근 제한자 순서
- 접근 제한자는 public, private 등 설정된 순서를 따름.
~~~
// 접근 제한자와 기타 수식자는 설정된 순서대로 작성
public static async Task ExampleMethod()
{
    await Task.Delay(1000);
}

private readonly int _value;
~~~

### var 사용 규칙
- 내장형 타입(예: int, string)에는 var 사용 안함.
- 타입이 명확한 경우에도 var 대신 명시적 타입 사용.
~~~
// 명시적 타입 사용
int number = 10;
string name = "John";

// 비권장
var number = 10;
var name = "John";
~~~

### 프로퍼티와 필드
- 가능한 경우 자동 프로퍼티를 사용
~~~
// 자동 프로퍼티 사용
public string Name { get; set; }

// 비권장 (필드를 직접 노출)
public string name;
~~~

### 네임스페이스 및 using 지시문
- using 지시문은 네임스페이스 외부에 배치.
- System 네임스페이스는 항상 다른 네임스페이스보다 먼저 작성.

~~~
// 권장
using System;
using System.Collections.Generic;

namespace ExampleNamespace
{
    public class ExampleClass
    {
    }
}

// 비권장
namespace ExampleNamespace
{
    using System;
    using System.Collections.Generic;

    public class ExampleClass
    {
    }
}
~~~


### 객체/컬렉션 초기화 및 표현식 관련 규칙
- 가능한 경우 객체 초기화 및 컬렉션 초기화 구문을 사용
~~~
// 객체 초기화 구문
var person = new Person
{
    Name = "John",
    Age = 30
};

// 컬렉션 초기화 구문
var numbers = new List<int> { 1, 2, 3 };

// 표현식 본문 사용 (권장)
public int Add(int x, int y) => x + y;

// 비권장 (블록 본문)
public int Add(int x, int y)
{
    return x + y;
}
~~~

### 패턴 매칭 및 조건식
- is 및 패턴 매칭을 선호.
- 가능한 경우 Switch 식 사용.
~~~
// 패턴 매칭 (권장)
if (obj is string text)
{
    Console.WriteLine(text);
}

// 비권장
if (obj is string)
{
    var text = (string)obj;
    Console.WriteLine(text);
}

// Switch 식 사용 (권장)
var result = input switch
{
    1 => "One",
    2 => "Two",
    _ => "Other"
};

// 비권장
string result;
switch (input)
{
    case 1:
        result = "One";
        break;
    case 2:
        result = "Two";
        break;
    default:
        result = "Other";
        break;
}
~~~

### 공백 스타일
캐스팅 후 공백 없음.
쉼표 뒤 공백 있음.
이진 연산자(+, - 등) 주위에 공백 있음.
~~~
// 캐스팅 후 공백 없음 (권장)
var result = (int)obj;

// 쉼표 뒤 공백 있음 (권장)
var list = new[] { 1, 2, 3 };

// 이진 연산자 주위 공백 있음 (권장)
var sum = a + b;

// 비권장
var result = (int) obj;
var list = new[] {1,2,3};
var sum = a+b;
~~~

### Null 검사 및 조건식
- 가능한 경우 ?? 연산자와 null 조건부 연산자(?.)를 사용.
- is null 검사를 선호.

~~~
// Null 병합 연산자 (권장)
var name = input ?? "Default";

// Null 조건부 연산자 (권장)
var length = input?.Length;

// Null 체크 (권장)
if (input is null)
{
    Console.WriteLine("Input is null");
}

// 비권장
var name = input != null ? input : "Default";
if (object.ReferenceEquals(input, null))
{
    Console.WriteLine("Input is null");
}
~~~

### 기타 규칙
- this 키워드는 필요하지 않은 경우 사용하지 않음.
~~~
// 권장
fieldName = value;

// 비권장
this.fieldName = value;
~~~


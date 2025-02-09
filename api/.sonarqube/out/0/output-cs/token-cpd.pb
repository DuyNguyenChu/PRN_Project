�
TD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Services\TokenService.cs
	namespace 	
api
 
. 
Services 
{ 
public 

class 
TokenService 
: 
ITokenService  -
{ 
private 
readonly 
IConfiguration '
_config( /
;/ 0
private 
readonly  
SymmetricSecurityKey -
_key. 2
;2 3
public 
TokenService 
( 
IConfiguration *
config+ 1
)1 2
{ 	
_config 
= 
config 
; 
_key 
= 
new  
SymmetricSecurityKey +
(+ ,
Encoding, 4
.4 5
UTF85 9
.9 :
GetBytes: B
(B C
_configC J
[J K
$strK [
][ \
)\ ]
)] ^
;^ _
} 	
public 
string 
CreateToken !
(! "
AppUser" )
user* .
). /
{ 	
var 
claims 
= 
new 
List !
<! "
Claim" '
>' (
{ 
new 
Claim 
( #
JwtRegisteredClaimNames 1
.1 2
Email2 7
,7 8
user9 =
.= >
Email> C
)C D
,D E
new 
Claim 
( #
JwtRegisteredClaimNames 1
.1 2
	GivenName2 ;
,; <
user= A
.A B
UserNameB J
)J K
} 
; 
var   
creds   
=   
new   
SigningCredentials   .
(  . /
_key  / 3
,  3 4
SecurityAlgorithms  5 G
.  G H
HmacSha512Signature  H [
)  [ \
;  \ ]
var"" 
tokenDescriptor"" 
=""  !
new""" %#
SecurityTokenDescriptor""& =
{## 
Subject$$ 
=$$ 
new$$ 
ClaimsIdentity$$ ,
($$, -
claims$$- 3
)$$3 4
,$$4 5
Expires%% 
=%% 
DateTime%% "
.%%" #
Now%%# &
.%%& '
AddDays%%' .
(%%. /
$num%%/ 0
)%%0 1
,%%1 2
SigningCredentials&& "
=&&# $
creds&&% *
,&&* +
Issuer'' 
='' 
_config''  
[''  !
$str''! -
]''- .
,''. /
Audience(( 
=(( 
_config(( "
[((" #
$str((# 1
]((1 2
,((2 3
}** 
;** 
var,, 
tokenHandler,, 
=,, 
new,, "#
JwtSecurityTokenHandler,,# :
(,,: ;
),,; <
;,,< =
var.. 
token.. 
=.. 
tokenHandler.. $
...$ %
CreateToken..% 0
(..0 1
tokenDescriptor..1 @
)..@ A
;..A B
return00 
tokenHandler00 
.00  

WriteToken00  *
(00* +
token00+ 0
)000 1
;001 2
}11 	
}22 
}33 �Q
VD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Services\ProblemService.cs
	namespace 	
api
 
. 
Services 
{ 
public 

class 
ProblemService 
:  !
IProblemService" 1
{ 
private 
readonly 
IProblemRepository +
_problemRepository, >
;> ?
private 
readonly !
ISubmissionRepository .!
_submissionRepository/ D
;D E
public 
ProblemService 
( 
IProblemRepository 0
problemRepository1 B
,B C!
ISubmissionRepositoryD Y 
submissionRepositoryZ n
)n o
{ 	
_problemRepository 
=  
problemRepository! 2
;2 3!
_submissionRepository !
=" # 
submissionRepository$ 8
;8 9
} 	
public 
async 
Task 
< 
List 
< 
string %
>% &
>& '!
GetAllCategoriesAsync( =
(= >
)> ?
{ 	
var 
problems 
= 
await  
_problemRepository! 3
.3 4
GetAllProblemAsync4 F
(F G
)G H
;H I
var 

categories 
= 
problems %
. 
Select 
( 
p 
=> 
p 
. 
Category '
)' (
. 
Where 
( 
c 
=> 
! 
string #
.# $
IsNullOrWhiteSpace$ 6
(6 7
c7 8
)8 9
)9 :
.: ;
Distinct; C
(C D
)D E
. 
ToList 
( 
) 
; 
return 

categories 
; 
} 	
public!! 
async!! 
Task!! 
<!! 

PageResult!! $
<!!$ %
ViewAllProblemDto!!% 6
>!!6 7
>!!7 8(
GetAllProblemsWithStatsAsync!!9 U
(!!U V
string!!V \
userId!!] c
,!!c d
QueryObject!!e p
query!!q v
)!!v w
{"" 	
var## 
problemsQuery## 
=## 
(##  !
await##! &
_problemRepository##' 9
.##9 :
GetAllProblemAsync##: L
(##L M
)##M N
)##N O
.##O P
AsQueryable##P [
(##[ \
)##\ ]
;##] ^
if%% 
(%% 
!%% 
string%% 
.%% 
IsNullOrWhiteSpace%% *
(%%* +
query%%+ 0
.%%0 1
ProblemTitle%%1 =
)%%= >
)%%> ?
{&& 
problemsQuery'' 
='' 
problemsQuery''  -
.''- .
Where''. 3
(''3 4
p''4 5
=>''6 8
p''9 :
.'': ;
Title''; @
.''@ A
Contains''A I
(''I J
query''J O
.''O P
ProblemTitle''P \
,''\ ]
StringComparison''^ n
.''n o
OrdinalIgnoreCase	''o �
)
''� �
)
''� �
;
''� �
}(( 
if** 
(** 
!** 
string** 
.** 
IsNullOrWhiteSpace** *
(*** +
query**+ 0
.**0 1
ProblemCategory**1 @
)**@ A
)**A B
{++ 
problemsQuery,, 
=,, 
problemsQuery,,  -
.,,- .
Where,,. 3
(,,3 4
p,,4 5
=>,,6 8
p,,9 :
.,,: ;
Category,,; C
.,,C D
Equals,,D J
(,,J K
query,,K P
.,,P Q
ProblemCategory,,Q `
,,,` a
StringComparison,,b r
.,,r s
OrdinalIgnoreCase	,,s �
)
,,� �
)
,,� �
;
,,� �
}-- 
var.. 
problems.. 
=.. 
new.. 
List.. #
<..# $
ViewAllProblemDto..$ 5
>..5 6
(..6 7
)..7 8
;..8 9
foreach// 
(// 
var// 
problem//  
in//! #
problemsQuery//$ 1
)//1 2
{00 
var11 
submissions11 
=11  !
await11" '!
_submissionRepository11( =
.11= >*
GetSubmissionsByProblemIdAsync11> \
(11\ ]
problem11] d
.11d e
	ProblemId11e n
)11n o
;11o p
var22 

problemDto22 
=22  
ProblemMapper22! .
.22. /
ToViewAllProblemDto22/ B
(22B C
problem22C J
,22J K
submissions22L W
,22W X
userId22Y _
)22_ `
;22` a
problems33 
.33 
Add33 
(33 

problemDto33 '
)33' (
;33( )
}44 
if66 
(66 
!66 
string66 
.66 
IsNullOrWhiteSpace66 *
(66* +
query66+ 0
.660 1
SortBy661 7
)667 8
)668 9
{77 
problems88 
=88 
query88  
.88  !
SortBy88! '
switch88( .
{99 
$str:: 
=>:: 
query:: !
.::! "
IsDescending::" .
.::. /
Equals::/ 5
(::5 6
$str::6 <
)::< =
?::> ?
problems::@ H
.::H I
OrderByDescending::I Z
(::Z [
p::[ \
=>::] _
p::` a
.::a b
AcceptanceRate::b p
)::p q
.::q r
ToList::r x
(::x y
)::y z
:;;< =
problems;;> F
.;;F G
OrderBy;;G N
(;;N O
p;;O P
=>;;Q S
p;;T U
.;;U V
AcceptanceRate;;V d
);;d e
.;;e f
ToList;;f l
(;;l m
);;m n
,;;n o
$str<< 
=><< 
query<< !
.<<! "
IsDescending<<" .
.<<. /
Equals<</ 5
(<<5 6
$str<<6 <
)<<< =
?<<> ?
problems<<@ H
.<<H I
OrderByDescending<<I Z
(<<Z [
p<<[ \
=><<] _
p<<` a
.<<a b
AcceptedCount<<b o
)<<o p
.<<p q
ToList<<q w
(<<w x
)<<x y
:==< =
problems==> F
.==F G
OrderBy==G N
(==N O
p==O P
=>==Q S
p==T U
.==U V
AcceptedCount==V c
)==c d
.==d e
ToList==e k
(==k l
)==l m
,==m n
$str>> 
=>>> 
query>>  
.>>  !
IsDescending>>! -
.>>- .
Equals>>. 4
(>>4 5
$str>>5 ;
)>>; <
?>>= >
problems>>? G
.>>G H
OrderByDescending>>H Y
(>>Y Z
p>>Z [
=>>>\ ^
p>>_ `
.>>` a
Score>>a f
)>>f g
.>>g h
ToList>>h n
(>>n o
)>>o p
:??( )
problems??* 2
.??2 3
OrderBy??3 :
(??: ;
p??; <
=>??= ?
p??@ A
.??A B
Score??B G
)??G H
.??H I
ToList??I O
(??O P
)??P Q
,??Q R
_@@ 
=>@@ 
problems@@ !
}AA 
;AA 
}BB 
varCC 
resultCC 
=CC 
newCC 
ListCC !
<CC! "
ViewAllProblemDtoCC" 3
>CC3 4
(CC4 5
)CC5 6
;CC6 7
intEE 

totalItemsEE 
=EE 
problemsEE %
.EE% &
CountEE& +
(EE+ ,
)EE, -
;EE- .
foreachGG 
(GG 
varGG 
problemGG  
inGG! #
problemsGG$ ,
.GG, -
SkipGG- 1
(GG1 2
(GG2 3
queryGG3 8
.GG8 9

PageNumberGG9 C
-GGD E
$numGGF G
)GGG H
*GGI J
queryGGK P
.GGP Q
PageSizeGGQ Y
)GGY Z
.GGZ [
TakeGG[ _
(GG_ `
queryGG` e
.GGe f
PageSizeGGf n
)GGn o
)GGo p
{HH 
boolJJ 

hidePassedJJ 
=JJ  !
falseJJ" '
;JJ' (
ifKK 
(KK 
!KK 
stringKK 
.KK 
IsNullOrWhiteSpaceKK .
(KK. /
queryKK/ 4
.KK4 5

HidePassedKK5 ?
)KK? @
)KK@ A
{LL 
boolMM 
.MM 
TryParseMM !
(MM! "
queryMM" '
.MM' (

HidePassedMM( 2
,MM2 3
outMM4 7

hidePassedMM8 B
)MMB C
;MMC D
}NN 
ifOO 
(OO 

hidePassedOO 
&&OO !
problemOO" )
.OO) *
SolvedStatusOO* 6
.OO6 7
EqualsOO7 =
(OO= >
$strOO> F
)OOF G
)OOG H
{PP 
continueQQ 
;QQ 
}RR 
resultSS 
.SS 
AddSS 
(SS 
problemSS "
)SS" #
;SS# $
}TT 
returnUU 
newUU 

PageResultUU !
<UU! "
ViewAllProblemDtoUU" 3
>UU3 4
{VV 
ItemsWW 
=WW 
resultWW 
,WW 

TotalItemsXX 
=XX 

totalItemsXX '
,XX' (

TotalPagesYY 
=YY 
(YY 
intYY !
)YY! "
MathYY" &
.YY& '
CeilingYY' .
(YY. /

totalItemsYY/ 9
/YY: ;
(YY< =
doubleYY= C
)YYC D
queryYYD I
.YYI J
PageSizeYYJ R
)YYR S
,YYS T
CurrentPageZZ 
=ZZ 
queryZZ #
.ZZ# $

PageNumberZZ$ .
}[[ 
;[[ 
}\\ 	
}]] 
}^^ �*
_D:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Services\ProblemHomePageServices.cs
	namespace		 	
api		
 
.		 
Services		 
{

 
public 

class #
ProblemHomePageServices (
:) *$
IProblemHomePageServices+ C
{ 
private 
readonly !
ISubmissionRepository .
_submissionRepo/ >
;> ?
public #
ProblemHomePageServices &
(& '!
ISubmissionRepository' <
submissionRepo= K
)K L
{ 	
_submissionRepo 
= 
submissionRepo ,
;, -
} 	
public 
async 
Task 
< 
List 
< %
ProblemHomePageNotDoneDto 8
?8 9
>9 :
>: ;&
GetXProblemAreNotDoneAsync< V
(V W
intW Z
pageSize[ c
,c d
stringe k
userIdl r
,r s
intt w
monthx }
,} ~
int	 �
year
� �
)
� �
{ 	
var 
submissions 
= 
await #
_submissionRepo$ 3
.3 4(
GetAllSubmissionAtMonthAsync4 P
(P Q
monthQ V
,V W
yearX \
)\ ]
;] ^
if 
( 
submissions 
== 
null #
)# $
return 
null 
; 
var 
problems 
= 
submissions &
.& '
Where' ,
(, -
x- .
=>/ 1
x2 3
.3 4
AppUser4 ;
.; <
Id< >
.> ?
Equals? E
(E F
userIdF L
)L M
&&N P
!Q R
xR S
.S T
StatusT Z
.Z [
Equals[ a
(a b
$strb l
)l m
)m n
.  !
Take! %
(% &
pageSize& .
). /
.  !
Select! '
(' (
x( )
=>* ,
x- .
.. /
Problem/ 6
.6 7/
#ToProblemHomePageNotDoneFromProblem7 Z
(Z [
([ \
x\ ]
.] ^
Point^ c
,c d
xe f
.f g
Statusg m
)m n
)n o
)o p
.  !
ToList! '
(' (
)( )
;) *
return 
problems 
; 
}   	
public"" 
async"" 
Task"" 
<"" 
List"" 
<"" *
ProblemHomePageMostAttempedDto"" =
?""= >
>""> ?
>""? @/
#GetXProblemHomePageMostAttmpedAsync""A d
(""d e
int""e h
pageSize""i q
,""q r
int""s v
month""w |
,""| }
int	""~ �
year
""� �
)
""� �
{## 	
var$$ 
submissions$$ 
=$$ 
await$$ #
_submissionRepo$$$ 3
.$$3 4(
GetAllSubmissionAtMonthAsync$$4 P
($$P Q
month$$Q V
,$$V W
year$$X \
)$$\ ]
;$$] ^
if&& 
(&& 
submissions&& 
==&& 
null&& #
)&&# $
return'' 
null'' 
;'' 
var)) 
problemExistTime))  
=))! "
submissions))# .
.** 
GroupBy** 
(** 
x** 
=>** 
x** 
.**  
Problem**  '
.**' (7
+ToProblemHomePageMostAttempedDtoFromProblem**( S
(**S T
)**T U
)**U V
.++ 
ToDictionary++ 
(++ 
g,, 
=>,, 
g,, 
.,, 
Key,, 
,,, 
g-- 
=>-- 
(-- 
g.. 
... 
Count.. 
(..  
)..  !
,..! "
g// 
.// 
Count// 
(//  
x//  !
=>//" $
x//% &
.//& '
Status//' -
.//- .
Equals//. 4
(//4 5
$str//5 ?
)//? @
)//@ A
)00 
)11 
;11 
var33 
topX33 
=33 
problemExistTime33 '
.33' (
OrderByDescending33( 9
(339 :
x33: ;
=>33< >
x33? @
.33@ A
Value33A F
)33F G
.33G H
Take33H L
(33L M
pageSize33M U
)33U V
.33V W
ToList33W ]
(33] ^
)33^ _
;33_ `
List55 
<55 *
ProblemHomePageMostAttempedDto55 /
>55/ 0
problems551 9
=55: ;
topX55< @
.55@ A
Select55A G
(55G H
x55H I
=>55J L
x55M N
.55N O
Key55O R
.55R S
AddNumAttempted55S b
(55b c
x55c d
.55d e
Value55e j
)55j k
)55k l
.55l m
ToList55m s
(55s t
)55t u
;55u v
return77 
problems77 
;77 
}88 	
}99 
}:: �
^D:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Repository\SubmissionRepository.cs
	namespace

 	
api


 
.

 

Repository

 
{ 
public 

class  
SubmissionRepository %
:& '!
ISubmissionRepository( =
{ 
private 
readonly  
ApplicationDBContext -
_context. 6
;6 7
public  
SubmissionRepository #
(# $ 
ApplicationDBContext$ 8
context9 @
)@ A
{ 	
_context 
= 
context 
; 
} 	
public 
async 
Task 
< 
List 
< 

Submission )
>) *
>* +(
GetAllSubmissionAtMonthAsync, H
(H I
intI L
monthM R
,R S
intT W
yearX \
)\ ]
{ 	
return 
await 
_context !
.! "
Submissions" -
.- .
Where. 3
(3 4
x4 5
=>6 8
x9 :
.: ;
SubmittedAt; F
.F G
YearG K
==L N
yearO S
&&T V
xW X
.X Y
SubmittedAtY d
.d e
Monthe j
==k m
monthn s
)s t
.t u
Includeu |
(| }
x} ~
=>	 �
x
� �
.
� �
Problem
� �
)
� �
.
� �
Include
� �
(
� �
x
� �
=>
� �
x
� �
.
� �
AppUser
� �
)
� �
.
� �
ToListAsync
� �
(
� �
)
� �
;
� �
} 	
public 
async 
Task 
< 
List 
< 

Submission )
>) *
>* +*
GetSubmissionsByProblemIdAsync, J
(J K
stringK Q
	problemIdR [
)[ \
{ 	
return 
await 
_context !
.! "
Submissions" -
.- .
Where. 3
(3 4
s4 5
=>6 8
s9 :
.: ;
Problem; B
.B C
	ProblemIdC L
.L M
EqualsM S
(S T
	problemIdT ]
)] ^
)^ _
._ `
ToListAsync` k
(k l
)l m
;m n
} 	
} 
} �
[D:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Repository\ProblemRepository.cs
	namespace 	
api
 
. 

Repository 
{ 
public 

class 
ProblemRepository "
:# $
IProblemRepository% 7
{ 
private 
readonly  
ApplicationDBContext -
_context. 6
;6 7
public 
ProblemRepository  
(  ! 
ApplicationDBContext! 5
context6 =
)= >
{ 	
_context 
= 
context 
; 
} 	
public 
async 
Task 
< 
List 
< 
Problem &
>& '
>' (
GetAllProblemAsync) ;
(; <
)< =
{ 	
return 
await 
_context !
.! "
Problems" *
.* +
ToListAsync+ 6
(6 7
)7 8
;8 9
} 	
} 
} �
cD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Repository\ProblemHomePageRepository.cs
	namespace 	
api
 
. 

Repository 
{		 
public

 

class

 %
ProblemHomePageRepository

 *
:

+ ,&
IProblemHomePageRepository

- G
{ 
private 
readonly  
ApplicationDBContext -
_context. 6
;6 7
public %
ProblemHomePageRepository (
(( ) 
ApplicationDBContext) =
context> E
)E F
{ 	
_context 
= 
context 
; 
} 	
} 
} �-
XD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Repository\BlogRepository.cs
	namespace 	
api
 
. 

Repository 
{ 
public 

class 
BlogRepository 
:  !
IBlogRepository" 1
{ 
private 
readonly  
ApplicationDBContext -
_Context. 6
;6 7
public 
BlogRepository 
(  
ApplicationDBContext 2
Context3 :
): ;
{ 	
_Context 
= 
Context 
; 
} 	
public 
async 
Task 
< 
Blog 
> 
CreateAsync  +
(+ ,
Blog, 0
	BlogModel1 :
): ;
{ 	
await 
_Context 
. 
Blogs  
.  !
AddAsync! )
() *
	BlogModel* 3
)3 4
;4 5
await 
_Context 
. 
SaveChangesAsync +
(+ ,
), -
;- .
return 
	BlogModel 
; 
} 	
public 
async 
Task 
< 
Blog 
? 
>  

DeleteAync! +
(+ ,
int, /
id0 2
)2 3
{ 	
var 
	BlogModel 
= 
await !
_Context" *
.* +
Blogs+ 0
.0 1
FirstOrDefaultAsync1 D
(D E
xE F
=>G I
xJ K
.K L
IDL N
==O Q
idR T
)T U
;U V
if   
(   
	BlogModel   
==   
null   !
)  ! "
{!! 
return"" 
null"" 
;"" 
}## 
_Context$$ 
.$$ 
Blogs$$ 
.$$ 
Remove$$ !
($$! "
	BlogModel$$" +
)$$+ ,
;$$, -
await%% 
_Context%% 
.%% 
SaveChangesAsync%% +
(%%+ ,
)%%, -
;%%- .
return&& 
	BlogModel&& 
;&& 
}'' 	
public)) 
async)) 
Task)) 
<)) 
List)) 
<)) 
Blog)) #
>))# $
>))$ %
GetAllAsync))& 1
())1 2
)))2 3
{** 	
return++ 
await++ 
_Context++ !
.++! "
Blogs++" '
.++' (
ToListAsync++( 3
(++3 4
)++4 5
;++5 6
},, 	
public.. 
async.. 
Task.. 
<.. 
Blog.. 
?.. 
>..  
GetByIDAsync..! -
(..- .
int... 1
id..2 4
)..4 5
{// 	
return00 
await00 
_Context00 !
.00! "
Blogs00" '
.00' (
FirstOrDefaultAsync00( ;
(00; <
i00< =
=>00> @
i00A B
.00B C
ID00C E
==00F H
id00I K
)00K L
;00L M
}11 	
public33 
async33 
Task33 
<33 
List33 
<33 
Blog33 #
>33# $
>33$ %
GetByUserIDAsync33& 6
(336 7
string337 =
id33> @
)33@ A
{44 	
return55 
await55 
_Context55 !
.55! "
Blogs55" '
.55' (
Where55( -
(55- .
i55. /
=>550 2
(553 4
i554 5
.555 6
UserId556 <
??55= ?
$str55@ B
)55B C
.55C D
Equals55D J
(55J K
id55K M
)55M N
)55N O
.55O P
ToListAsync55P [
(55[ \
)55\ ]
;55] ^
}66 	
public88 
async88 
Task88 
<88 
Blog88 
?88 
>88  

UpdateAync88! +
(88+ ,
int88, /
id880 2
,882 3
UpdateBlogRequesDto884 G
BlogDto88H O
)88O P
{99 	
var:: 
existingBlog:: 
=:: 
await:: $
_Context::% -
.::- .
Blogs::. 3
.::3 4
FirstOrDefaultAsync::4 G
(::G H
x::H I
=>::J L
x::M N
.::N O
ID::O Q
==::R T
id::U W
)::W X
;::X Y
if;; 
(;; 
existingBlog;; 
==;; 
null;;  $
);;$ %
{<< 
return== 
null== 
;== 
}>> 
existingBlog@@ 
.@@ 
	Thumbnail@@ "
=@@# $
BlogDto@@% ,
.@@, -
	Thumbnail@@- 6
;@@6 7
existingBlogAA 
.AA 
titleAA 
=AA  
BlogDtoAA! (
.AA( )
titleAA) .
;AA. /
existingBlogBB 
.BB 
descriptionBB $
=BB% &
BlogDtoBB' .
.BB. /
descriptionBB/ :
;BB: ;
existingBlogCC 
.CC 
ContentCC  
=CC! "
BlogDtoCC# *
.CC* +
ContentCC+ 2
;CC2 3
existingBlogDD 
.DD 
StatusDD 
=DD  !
BlogDtoDD" )
.DD) *
StatusDD* 0
;DD0 1
awaitGG 
_ContextGG 
.GG 
SaveChangesAsyncGG +
(GG+ ,
)GG, -
;GG- .
returnII 
existingBlogII 
;II  
}JJ 	
}KK 
}LL �P
FD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Program.cs
var 
builder 
= 
WebApplication 
. 
CreateBuilder *
(* +
args+ /
)/ 0
;0 1
builder 
. 
Services 
. #
AddEndpointsApiExplorer (
(( )
)) *
;* +
builder 
. 
Services 
. 
AddSwaggerGen 
( 
)  
;  !
builder 
. 
Services 
. 
AddControllers 
(  
)  !
;! "
builder 
. 
Services 
. 
AddSwaggerGen 
( 
option %
=>& (
{ 
option 

.
 

SwaggerDoc 
( 
$str 
, 
new 
OpenApiInfo  +
{, -
Title. 3
=4 5
$str6 @
,@ A
VersionB I
=J K
$strL P
}Q R
)R S
;S T
option 

.
 !
AddSecurityDefinition  
(  !
$str! )
,) *
new+ .!
OpenApiSecurityScheme/ D
{ 
In 

= 
ParameterLocation 
. 
Header %
,% &
Description 
= 
$str 2
,2 3
Name 
= 
$str 
, 
Type 
= 
SecuritySchemeType !
.! "
Http" &
,& '
BearerFormat 
= 
$str 
, 
Scheme 
= 
$str 
}   
)   
;   
option!! 

.!!
 "
AddSecurityRequirement!! !
(!!! "
new!!" %&
OpenApiSecurityRequirement!!& @
{"" 
{## 	
new$$ !
OpenApiSecurityScheme$$ %
{%% 
	Reference&& 
=&& 
new&& 
OpenApiReference&&  0
{'' 
Type(( 
=(( 
ReferenceType(( &
.((& '
SecurityScheme((' 5
,((5 6
Id)) 
=)) 
$str)) 
}** 
}++ 
,++ 
new,, 
string,, 
[,, 
],, 
{,, 
},, 
}-- 	
}.. 
).. 
;.. 
}// 
)// 
;// 
builder00 
.00 
Services00 
.00 
AddCors00 
(00 
options00  
=>00! #
{11 
options22 
.22 
	AddPolicy22 
(22 
$str22 %
,22% &
policy33 
=>33 
{44 	
policy55 
.55 
WithOrigins55 
(55 
$str55 6
)556 7
.66 
AllowAnyMethod66 !
(66! "
)66" #
.77 
AllowAnyHeader77 !
(77! "
)77" #
.88 
AllowCredentials88 #
(88# $
)88$ %
;88% &
}99 	
)99	 

;99
 
}:: 
):: 
;:: 
builder<< 
.<< 
Services<< 
.<< 
AddDbContext<< 
<<<  
ApplicationDBContext<< 2
><<2 3
(<<3 4
options<<4 ;
=><<< >
{== 
options>> 
.>> 
UseSqlServer>> 
(>> 
builder>>  
.>>  !
Configuration>>! .
.>>. /
GetConnectionString>>/ B
(>>B C
$str>>C V
)>>V W
)>>W X
;>>X Y
}?? 
)?? 
;?? 
builderBB 
.BB 
ServicesBB 
.BB 
AddIdentityBB 
<BB 
AppUserBB $
,BB$ %
IdentityRoleBB& 2
>BB2 3
(BB3 4
optionsBB4 ;
=>BB< >
{CC 
optionsDD 
.DD 
PasswordDD 
.DD 
RequireDigitDD !
=DD" #
trueDD$ (
;DD( )
optionsEE 
.EE 
PasswordEE 
.EE 
RequireLowercaseEE %
=EE& '
trueEE( ,
;EE, -
optionsFF 
.FF 
PasswordFF 
.FF 
RequireUppercaseFF %
=FF& '
trueFF( ,
;FF, -
optionsGG 
.GG 
PasswordGG 
.GG "
RequireNonAlphanumericGG +
=GG, -
trueGG. 2
;GG2 3
optionsHH 
.HH 
PasswordHH 
.HH 
RequiredLengthHH #
=HH$ %
$numHH& (
;HH( )
}JJ 
)JJ 
.KK $
AddEntityFrameworkStoresKK 
<KK  
ApplicationDBContextKK .
>KK. /
(KK/ 0
)KK0 1
.LL $
AddDefaultTokenProvidersLL 
(LL 
)LL 
;LL 
builderNN 
.NN 
ServicesNN 
.NN 
AddAuthenticationNN "
(NN" #
optionsNN# *
=>NN+ -
{OO 
optionsPP 
.PP %
DefaultAuthenticateSchemePP %
=PP& '
optionsQQ 
.QQ "
DefaultChallengeSchemeQQ "
=QQ# $
optionsRR 
.RR 
DefaultForbidSchemeRR 
=RR  !
optionsSS 
.SS 
DefaultSchemeSS 
=SS 
optionsTT 
.TT 
DefaultSignInSchemeTT 
=TT  !
optionsUU 
.UU  
DefaultSignOutSchemeUU  
=UU! "
JwtBearerDefaultsUU# 4
.UU4 5 
AuthenticationSchemeUU5 I
;UUI J
}VV 
)VV 
.VV 
AddJwtBearerVV 
(VV 
optionsVV 
=>VV 
{WW 
optionsXX 
.XX %
TokenValidationParametersXX %
=XX& '
newXX( +%
TokenValidationParametersXX, E
{YY 
ValidateIssuerZZ 
=ZZ 
trueZZ 
,ZZ 
ValidIssuer[[ 
=[[ 
builder[[ 
.[[ 
Configuration[[ +
[[[+ ,
$str[[, 8
][[8 9
,[[9 :
ValidateAudience\\ 
=\\ 
true\\ 
,\\  
ValidAudience]] 
=]] 
builder]] 
.]]  
Configuration]]  -
[]]- .
$str]]. <
]]]< =
,]]= >$
ValidateIssuerSigningKey^^  
=^^! "
true^^# '
,^^' (
IssuerSigningKey__ 
=__ 
new__  
SymmetricSecurityKey__ 3
(__3 4
System`` 
.`` 
Text`` 
.`` 
Encoding``  
.``  !
UTF8``! %
.``% &
GetBytes``& .
(``. /
builder``/ 6
.``6 7
Configuration``7 D
[``D E
$str``E U
]``U V
)``V W
)aa 	
}bb 
;bb 
}cc 
)cc 
;cc 
builderee 
.ee 
Servicesee 
.ee 
	AddScopedee 
<ee 
ITokenServiceee (
,ee( )
TokenServiceee* 6
>ee6 7
(ee7 8
)ee8 9
;ee9 :
builderff 
.ff 
Servicesff 
.ff 
	AddScopedff 
<ff !
ISubmissionRepositoryff 0
,ff0 1 
SubmissionRepositoryff2 F
>ffF G
(ffG H
)ffH I
;ffI J
buildergg 
.gg 
Servicesgg 
.gg 
	AddScopedgg 
<gg 
IProblemRepositorygg -
,gg- .
ProblemRepositorygg/ @
>gg@ A
(ggA B
)ggB C
;ggC D
builderhh 
.hh 
Serviceshh 
.hh 
	AddScopedhh 
<hh 
IProblemServicehh *
,hh* +
ProblemServicehh, :
>hh: ;
(hh; <
)hh< =
;hh= >
builderkk 
.kk 
Serviceskk 
.kk 
	AddScopedkk 
<kk &
IProblemHomePageRepositorykk 5
,kk5 6%
ProblemHomePageRepositorykk7 P
>kkP Q
(kkQ R
)kkR S
;kkS T
builderll 
.ll 
Servicesll 
.ll 
	AddScopedll 
<ll $
IProblemHomePageServicesll 3
,ll3 4#
ProblemHomePageServicesll5 L
>llL M
(llM N
)llN O
;llO P
builderoo 
.oo 
Servicesoo 
.oo 
	AddScopedoo 
<oo 
IBlogRepositoryoo *
,oo* +
BlogRepositoryoo, :
>oo: ;
(oo; <
)oo< =
;oo= >
varrr 
apprr 
=rr 	
builderrr
 
.rr 
Buildrr 
(rr 
)rr 
;rr 
ifuu 
(uu 
appuu 
.uu 
Environmentuu 
.uu 
IsDevelopmentuu !
(uu! "
)uu" #
)uu# $
{vv 
appww 
.ww 

UseSwaggerww 
(ww 
)ww 
;ww 
appxx 
.xx 
UseSwaggerUIxx 
(xx 
)xx 
;xx 
}yy 
app{{ 
.{{ 
UseCors{{ 
({{ 
$str{{ 
){{ 
;{{ 
app|| 
.|| 
UseHttpsRedirection|| 
(|| 
)|| 
;|| 
app}} 
.}} 
UseAuthentication}} 
(}} 
)}} 
;}} 
app~~ 
.~~ 
UseAuthorization~~ 
(~~ 
)~~ 
;~~ 
app 
. 
MapControllers 
( 
) 
; 
app�� 
.
�� 
Run
�� 
(
�� 
)
�� 	
;
��	 
�
SD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Model\TestCaseStatus.cs
	namespace 	
api
 
. 
Model 
{ 
public		 

class		 
TestCaseStatus		 
{

 
[ 	
Key	 
] 
public 
string 

TestCaseId  
{! "
get# &
;& '
set( +
;+ ,
}- .
=/ 0
string1 7
.7 8
Empty8 =
;= >
public 
double 
ExecutionTime #
{$ %
get& )
;) *
set+ .
;. /
}0 1
[ 	
Required	 
] 
public 
int 
MemoryUsage 
{  
get! $
;$ %
set& )
;) *
}+ ,
[ 	
Required	 
] 
[ 	
	MaxLength	 
( 
$num 
) 
] 
public 
string 
Result 
{ 
get "
;" #
set$ '
;' (
}) *
=+ ,
string- 3
.3 4
Empty4 9
;9 :
public 
string 
Log 
{ 
get 
;  
set! $
;$ %
}& '
=( )
string* 0
.0 1
Empty1 6
;6 7
public 

Submission 

Submission $
{% &
get' *
;* +
set, /
;/ 0
}1 2
=3 4
new5 8

Submission9 C
(C D
)D E
;E F
} 
} �
MD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Model\TestCase.cs
	namespace 	
api
 
. 
Model 
{		 
public

 

class

 
TestCase

 
{ 
[ 	
Key	 
] 
public 
string 

TestCaseId  
{! "
get# &
;& '
set( +
;+ ,
}- .
=/ 0
string1 7
.7 8
Empty8 =
;= >
[ 	
Required	 
] 
public 
string 
	ProblemId 
{  !
get" %
;% &
set' *
;* +
}, -
=. /
string0 6
.6 7
Empty7 <
;< =
[ 	
Required	 
, 
	MaxLength 
( 
$num  
)  !
]! "
public 
string 
TestCaseName "
{# $
get% (
;( )
set* -
;- .
}/ 0
=1 2
string3 9
.9 :
Empty: ?
;? @
[ 	
Required	 
] 
public 
string 
Input 
{ 
get !
;! "
set# &
;& '
}( )
=* +
string, 2
.2 3
Empty3 8
;8 9
[ 	
Required	 
] 
public 
string 
Output 
{ 
get "
;" #
set$ '
;' (
}) *
=+ ,
string- 3
.3 4
Empty4 9
;9 :
public 
Problem 
Problem 
{  
get! $
;$ %
set& )
;) *
}+ ,
=- .
new/ 2
Problem3 :
(: ;
); <
;< =
} 
} �
OD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Model\Submission.cs
	namespace 	
api
 
. 
Model 
{ 
public		 

class		 

Submission		 
{

 
[ 	
Key	 
] 
public 
string 
SubmissionId "
{# $
get% (
;( )
set* -
;- .
}/ 0
=1 2
string3 9
.9 :
Empty: ?
;? @
[ 	
Required	 
] 
[ 	
Range	 
( 
$num 
, 
$num 
) 
] 
public 
int 
Point 
{ 
get 
; 
set  #
;# $
}% &
[ 	
Required	 
] 
public 
string 

SourceCode  
{! "
get# &
;& '
set( +
;+ ,
}- .
=/ 0
string1 7
.7 8
Empty8 =
;= >
[ 	
Required	 
] 
[ 	
	MaxLength	 
( 
$num 
) 
] 
public 
string 
Status 
{ 
get "
;" #
set$ '
;' (
}) *
=+ ,
string- 3
.3 4
Empty4 9
;9 :
[ 	
Required	 
] 
public 
double 
ExecuteTime !
{" #
get$ '
;' (
set) ,
;, -
}. /
[ 	
Required	 
] 
public 
int 

MemoryUsed 
{ 
get  #
;# $
set% (
;( )
}* +
[ 	
Required	 
] 
[ 	
	MaxLength	 
( 
$num 
) 
] 
public 
string 
Language 
{  
get! $
;$ %
set& )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
[ 	
Required	 
] 
public 
DateTime 
SubmittedAt #
{$ %
get& )
;) *
set+ .
;. /
}0 1
=2 3
DateTime4 <
.< =
UtcNow= C
;C D
[ 	
Required	 
] 
public 
Problem 
Problem 
{  
get! $
;$ %
set& )
;) *
}+ ,
=- .
new/ 2
Problem3 :
(: ;
); <
;< =
public   
AppUser   
AppUser   
{    
get  ! $
;  $ %
set  & )
;  ) *
}  + ,
=  - .
new  / 2
AppUser  3 :
(  : ;
)  ; <
;  < =
}!! 
}"" �
LD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Model\Problem.cs
	namespace 	
api
 
. 
Model 
{ 
public		 

class		 
Problem		 
{

 
[ 	
Key	 
] 
public 
string 
	ProblemId 
{  !
get" %
;% &
set' *
;* +
}, -
=. /
string0 6
.6 7
Empty7 <
;< =
[ 	
Required	 
, 
	MaxLength 
( 
$num  
)  !
]! "
public 
string 
Category 
{  
get! $
;$ %
set& )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
[ 	
Required	 
, 
	MaxLength 
( 
$num  
)  !
]! "
public 
string 
Title 
{ 
get !
;! "
set# &
;& '
}( )
=* +
string, 2
.2 3
Empty3 8
;8 9
[ 	
Required	 
] 
public 
string 
Detail 
{ 
get "
;" #
set$ '
;' (
}) *
=+ ,
string- 3
.3 4
Empty4 9
;9 :
[ 	
Required	 
] 
public 
string 
Input 
{ 
get !
;! "
set# &
;& '
}( )
=* +
string, 2
.2 3
Empty3 8
;8 9
[ 	
Required	 
] 
public 
string 
Output 
{ 
get "
;" #
set$ '
;' (
}) *
=+ ,
string- 3
.3 4
Empty4 9
;9 :
[ 	
Range	 
( 
$num 
, 
$num 
) 
] 
public 
int 

TotalPoint 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
$num. /
;/ 0
[ 	
Required	 
] 
public 
int 
	TimeLimit 
{ 
get "
;" #
set$ '
;' (
}) *
[ 	
Required	 
] 
public 
int 
MemoryLimit 
{  
get! $
;$ %
set& )
;) *
}+ ,
[ 	
	MaxLength	 
( 
$num 
) 
] 
public 
string 
? 
Author 
{ 
get  #
;# $
set% (
;( )
}* +
public 
string 
? 
Solution 
{  !
get" %
;% &
set' *
;* +
}, -
public   
ICollection   
<   
ContestProblem   )
>  ) *
?  * +
ContestProblems  , ;
{  < =
get  > A
;  A B
set  C F
;  F G
}  H I
}!! 
}"" �
OD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Model\PageResult.cs
	namespace 	
api
 
. 
Model 
{ 
public 

class 

PageResult 
< 
T 
> 
{		 
public

 
List

 
<

 
T

 
>

 
?

 
Items

 
{

 
get

  #
;

# $
set

% (
;

( )
}

* +
public 
int 

TotalItems 
{ 
get  #
;# $
set% (
;( )
}* +
public 
int 

TotalPages 
{ 
get  #
;# $
set% (
;( )
}* +
public 
int 
CurrentPage 
{  
get! $
;$ %
set& )
;) *
}+ ,
} 
} �
UD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Model\ContestRegistion.cs
	namespace 	
api
 
. 
Model 
{ 
public		 

class		 
ContestRegistion		 !
{

 
[ 	
Required	 
] 
public 
string 
	ContestId 
{  !
get" %
;% &
set' *
;* +
}, -
=. /
string0 6
.6 7
Empty7 <
;< =
[ 	
Required	 
] 
public 
string 
Id 
{ 
get 
; 
set  #
;# $
}% &
=' (
string) /
./ 0
Empty0 5
;5 6
public 
DateTime 
	CreatedAt !
{" #
get$ '
;' (
set) ,
;, -
}. /
=0 1
DateTime2 :
.: ;
UtcNow; A
;A B
public 
Contest 
Contest 
{  
get! $
;$ %
set& )
;) *
}+ ,
=- .
new/ 2
Contest3 :
(: ;
); <
;< =
public 
AppUser 
AppUser 
{  
get! $
;$ %
set& )
;) *
}+ ,
=- .
new/ 2
AppUser3 :
(: ;
); <
;< =
} 
} �

SD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Model\ContestProblem.cs
	namespace 	
api
 
. 
Model 
{ 
public		 

class		 
ContestProblem		 
{

 
[ 
Required 
] 
public 

string 
	ContestId 
{ 
get !
;! "
set# &
;& '
}' (
=) *
string+ 1
.1 2
Empty2 7
;7 8
[ 
Required 
] 
public 

string 
	ProblemId 
{ 
get !
;! "
set# &
;& '
}' (
=) *
string+ 1
.1 2
Empty2 7
;7 8
public 

Contest 
Contest 
{ 
get  
;  !
set" %
;% &
}' (
=) *
new+ .
Contest/ 6
(6 7
)7 8
;8 9
public 

Problem 
Problem 
{ 
get  
;  !
set" %
;% &
}' (
=) *
new+ .
Problem/ 6
(6 7
)7 8
;8 9
} 
} �
LD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Model\Contest.cs
	namespace 	
api
 
. 
Model 
{ 
public		 

class		 
Contest		 
{

 
[ 	
Key	 
] 
public 
string 
	ContestId 
{  !
get" %
;% &
set' *
;* +
}, -
=. /
string0 6
.6 7
Empty7 <
;< =
[ 	
Required	 
] 
[ 	
	MaxLength	 
( 
$num 
) 
] 
public 
string 
ContestName !
{" #
get$ '
;' (
set) ,
;, -
}. /
=0 1
string2 8
.8 9
Empty9 >
;> ?
[ 	
Required	 
] 
public 
DateTime 
DueTime 
{  !
get" %
;% &
set' *
;* +
}, -
[ 	
Required	 
] 
public 
DateTime 
	CreatedAt !
{" #
get$ '
;' (
set) ,
;, -
}. /
=0 1
DateTime2 :
.: ;
UtcNow; A
;A B
[ 	
Required	 
] 
public 
int 

TotalPoint 
{ 
get  #
;# $
set% (
;( )
}* +
[ 	
Required	 
] 
public 
int 
Level 
{ 
get 
; 
set  #
;# $
}% &
[ 	
Required	 
] 
public 
DateTime 
EndDate 
{  !
get" %
;% &
set' *
;* +
}, -
public 
ICollection 
< 
ContestProblem )
>) *
?* +
ContestProblems, ;
{< =
get> A
;A B
setC F
;F G
}H I
} 
} �
PD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Model\CommentBlog.cs
	namespace 	
api
 
. 
Model 
{ 
public 

class 
CommentBlog 
{		 
public

 
int

 
ID

 
{

 
get

 
;

 
set

  
;

  !
}

" #
public 
string 
title 
{ 
get !
;! "
set# &
;& '
}( )
=* +
string, 2
.2 3
Empty3 8
;8 9
public 
string 
content 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
string. 4
.4 5
Empty5 :
;: ;
public 
DateTime 
CreateOn  
{! "
get# &
;& '
set( +
;+ ,
}- .
=/ 0
DateTime1 9
.9 :
Now: =
;= >
public 
int 
? 
BlogId 
{ 
get  
;  !
set" %
;% &
}' (
public 
Blog 
? 
Blog 
{ 
get 
;  
set! $
;$ %
}& '
} 
} �
LD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Model\Comment.cs
	namespace 	
api
 
. 
Model 
{ 
public		 

class		 
Comment		 
{

 
[ 	
Key	 
] 
public 
string 
	CommentId 
{  !
get" %
;% &
set' *
;* +
}, -
=. /
string0 6
.6 7
Empty7 <
;< =
[ 	
Required	 
] 
[ 	
	MaxLength	 
( 
$num 
) 
] 
public 
string 
Content 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
string. 4
.4 5
Empty5 :
;: ;
[ 	
Required	 
] 
public 
DateTime 
	CreatedAt !
{" #
get$ '
;' (
set) ,
;, -
}. /
=0 1
DateTime2 :
.: ;
UtcNow; A
;A B
[ 	
Required	 
] 
public 
DateTime 
	UpdatedAt !
{" #
get$ '
;' (
set) ,
;, -
}. /
=0 1
DateTime2 :
.: ;
UtcNow; A
;A B
public 
AppUser 
User 
{ 
get !
;! "
set# &
;& '
}( )
=* +
new, /
AppUser0 7
(7 8
)8 9
;9 :
public 
Problem 
Problem 
{  
get! $
;$ %
set& )
;) *
}+ ,
=- .
new/ 2
Problem3 :
(: ;
); <
;< =
} 
} �
ID:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Model\Blog.cs
	namespace		 	
api		
 
.		 
Model		 
{

 
public 

class 
Blog 
{ 
public 
int 
ID 
{ 
get 
; 
set  
;  !
}" #
public 
string 
? 
UserId 
{ 
get  #
;# $
set% (
;( )
}* +
public 
AppUser 
? 
User 
{ 
get "
;" #
set$ '
;' (
}) *
public 
string 
? 
	GuestName  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 
string 
? 

GuestEmail !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
string 
	Thumbnail 
{  !
get" %
;% &
set' *
;* +
}, -
=. /
string0 6
.6 7
Empty7 <
;< =
public 
string 
title 
{ 
get !
;! "
set# &
;& '
}( )
=* +
string, 2
.2 3
Empty3 8
;8 9
[ 	
StringLength	 
( 
$num 
, 
ErrorMessage %
=& '
$str( N
)N O
]O P
public 
string 
description !
{" #
get$ '
;' (
set) ,
;, -
}. /
=0 1
string2 8
.8 9
Empty9 >
;> ?
public 
string 
Content 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
string. 4
.4 5
Empty5 :
;: ;
public 
string 
	ImageBlog 
{  !
get" %
;% &
set' *
;* +
}, -
=. /
string0 6
.6 7
Empty7 <
;< =
public 
string 
CategoryBlog "
{# $
get% (
;( )
set* -
;- .
}/ 0
=1 2
string3 9
.9 :
Empty: ?
;? @
public 
string 
Status 
{ 
get "
;" #
set$ '
;' (
}) *
=+ ,
string- 3
.3 4
Empty4 9
;9 :
public 
DateTime 
CreateOn  
{! "
get# &
;& '
set( +
;+ ,
}- .
=/ 0
DateTime1 9
.9 :
Now: =
;= >
public 
DateTime 
? 

DatePublic #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public   
string   
TagBlog   
{   
get    #
;  # $
set  % (
;  ( )
}  * +
=  , -
string  . 4
.  4 5
Empty  5 :
;  : ;
public!! 
List!! 
<!! 
CommentBlog!! 
>!!  
CommentBlog!!! ,
{!!- .
get!!/ 2
;!!2 3
set!!4 7
;!!7 8
}!!9 :
=!!; <
new!!= @
List!!A E
<!!E F
CommentBlog!!F Q
>!!Q R
(!!R S
)!!S T
;!!T U
}"" 
}## �
LD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Model\AppUser.cs
	namespace 	
api
 
. 
Model 
{ 
public		 

class		 
AppUser		 
:		 
IdentityUser		 '
{

 
public 
string 
? 
FullName 
{  !
get" %
;% &
set' *
;* +
}, -
public 
string 
? 
Address 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
string 
? 
Avatar 
{ 
get  #
;# $
set% (
;( )
}* +
public 
DateTime 
? 
DateOfBirth $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 
string 
? 
PhoneNumber "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 
DateTime 
	CreatedAt !
{" #
get$ '
;' (
set) ,
;, -
}. /
=0 1
DateTime2 :
.: ;
UtcNow; A
;A B
public 
DateTime 
? 
LastLoginAt $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 
int 
TotalSolved 
{  
get! $
;$ %
set& )
;) *
}+ ,
=- .
$num/ 0
;0 1
public 
int 
TotalSubmissions #
{$ %
get& )
;) *
set+ .
;. /
}0 1
=2 3
$num4 5
;5 6
public 
DateTime 
? 
LastSolvedAt %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
} 
} ڼ
`D:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Migrations\20250208065413_FixBlog.cs
	namespace 	
api
 
. 

Migrations 
{		 
public 

partial 
class 
FixBlog  
:! "
	Migration# ,
{ 
	protected 
override 
void 
Up  "
(" #
MigrationBuilder# 3
migrationBuilder4 D
)D E
{ 	
migrationBuilder 
. 
DropForeignKey +
(+ ,
name 
: 
$str ,
,, -
table 
: 
$str 
) 
;  
migrationBuilder 
. 
	DropTable &
(& '
name 
: 
$str $
)$ %
;% &
migrationBuilder 
. 
	DropTable &
(& '
name 
: 
$str !
)! "
;" #
migrationBuilder 
. 
	DropTable &
(& '
name 
: 
$str 
)  
;  !
migrationBuilder 
. 
	DropTable &
(& '
name 
: 
$str 
) 
; 
migrationBuilder   
.   

DeleteData   '
(  ' (
table!! 
:!! 
$str!! $
,!!$ %
	keyColumn"" 
:"" 
$str"" 
,""  
keyValue## 
:## 
$str## @
)##@ A
;##A B
migrationBuilder%% 
.%% 

DeleteData%% '
(%%' (
table&& 
:&& 
$str&& $
,&&$ %
	keyColumn'' 
:'' 
$str'' 
,''  
keyValue(( 
:(( 
$str(( @
)((@ A
;((A B
migrationBuilder** 
.** 

DeleteData** '
(**' (
table++ 
:++ 
$str++ $
,++$ %
	keyColumn,, 
:,, 
$str,, 
,,,  
keyValue-- 
:-- 
$str-- @
)--@ A
;--A B
migrationBuilder// 
.// 
AlterColumn// (
<//( )
string//) /
>/// 0
(//0 1
name00 
:00 
$str00 
,00 
table11 
:11 
$str11 
,11 
type22 
:22 
$str22 %
,22% &
nullable33 
:33 
true33 
,33 

oldClrType44 
:44 
typeof44 "
(44" #
int44# &
)44& '
,44' (
oldType55 
:55 
$str55 
,55 
oldNullable66 
:66 
true66 !
)66! "
;66" #
migrationBuilder88 
.88 
	AddColumn88 &
<88& '
string88' -
>88- .
(88. /
name99 
:99 
$str99 $
,99$ %
table:: 
::: 
$str:: 
,:: 
type;; 
:;; 
$str;; %
,;;% &
nullable<< 
:<< 
false<< 
,<<  
defaultValue== 
:== 
$str==  
)==  !
;==! "
migrationBuilder?? 
.?? 
	AddColumn?? &
<??& '
string??' -
>??- .
(??. /
name@@ 
:@@ 
$str@@ !
,@@! "
tableAA 
:AA 
$strAA 
,AA 
typeBB 
:BB 
$strBB %
,BB% &
nullableCC 
:CC 
falseCC 
,CC  
defaultValueDD 
:DD 
$strDD  
)DD  !
;DD! "
migrationBuilderFF 
.FF 
	AddColumnFF &
<FF& '
stringFF' -
>FF- .
(FF. /
nameGG 
:GG 
$strGG 
,GG  
tableHH 
:HH 
$strHH 
,HH 
typeII 
:II 
$strII %
,II% &
nullableJJ 
:JJ 
falseJJ 
,JJ  
defaultValueKK 
:KK 
$strKK  
)KK  !
;KK! "
migrationBuilderMM 
.MM 

InsertDataMM '
(MM' (
tableNN 
:NN 
$strNN $
,NN$ %
columnsOO 
:OO 
newOO 
[OO 
]OO 
{OO  
$strOO! %
,OO% &
$strOO' 9
,OO9 :
$strOO; A
,OOA B
$strOOC S
}OOT U
,OOU V
valuesPP 
:PP 
newPP 
objectPP "
[PP" #
,PP# $
]PP$ %
{QQ 
{RR 
$strRR <
,RR< =
nullRR> B
,RRB C
$strRRD K
,RRK L
$strRRM T
}RRU V
,RRV W
{SS 
$strSS <
,SS< =
nullSS> B
,SSB C
$strSSD K
,SSK L
$strSSM T
}SSU V
,SSV W
{TT 
$strTT <
,TT< =
nullTT> B
,TTB C
$strTTD J
,TTJ K
$strTTL R
}TTS T
}UU 
)UU 
;UU 
migrationBuilderWW 
.WW 
AddForeignKeyWW *
(WW* +
nameXX 
:XX 
$strXX 3
,XX3 4
tableYY 
:YY 
$strYY 
,YY 
columnZZ 
:ZZ 
$strZZ  
,ZZ  !
principalTable[[ 
:[[ 
$str[[  -
,[[- .
principalColumn\\ 
:\\  
$str\\! %
)\\% &
;\\& '
}]] 	
	protected`` 
override`` 
void`` 
Down``  $
(``$ %
MigrationBuilder``% 5
migrationBuilder``6 F
)``F G
{aa 	
migrationBuilderbb 
.bb 
DropForeignKeybb +
(bb+ ,
namecc 
:cc 
$strcc 3
,cc3 4
tabledd 
:dd 
$strdd 
)dd 
;dd  
migrationBuilderff 
.ff 

DeleteDataff '
(ff' (
tablegg 
:gg 
$strgg $
,gg$ %
	keyColumnhh 
:hh 
$strhh 
,hh  
keyValueii 
:ii 
$strii @
)ii@ A
;iiA B
migrationBuilderkk 
.kk 

DeleteDatakk '
(kk' (
tablell 
:ll 
$strll $
,ll$ %
	keyColumnmm 
:mm 
$strmm 
,mm  
keyValuenn 
:nn 
$strnn @
)nn@ A
;nnA B
migrationBuilderpp 
.pp 

DeleteDatapp '
(pp' (
tableqq 
:qq 
$strqq $
,qq$ %
	keyColumnrr 
:rr 
$strrr 
,rr  
keyValuess 
:ss 
$strss @
)ss@ A
;ssA B
migrationBuilderuu 
.uu 

DropColumnuu '
(uu' (
namevv 
:vv 
$strvv $
,vv$ %
tableww 
:ww 
$strww 
)ww 
;ww  
migrationBuilderyy 
.yy 

DropColumnyy '
(yy' (
namezz 
:zz 
$strzz !
,zz! "
table{{ 
:{{ 
$str{{ 
){{ 
;{{  
migrationBuilder}} 
.}} 

DropColumn}} '
(}}' (
name~~ 
:~~ 
$str~~ 
,~~  
table 
: 
$str 
) 
;  
migrationBuilder
�� 
.
�� 
AlterColumn
�� (
<
��( )
int
��) ,
>
��, -
(
��- .
name
�� 
:
�� 
$str
�� 
,
�� 
table
�� 
:
�� 
$str
�� 
,
�� 
type
�� 
:
�� 
$str
�� 
,
�� 
nullable
�� 
:
�� 
true
�� 
,
�� 

oldClrType
�� 
:
�� 
typeof
�� "
(
��" #
string
��# )
)
��) *
,
��* +
oldType
�� 
:
�� 
$str
�� (
,
��( )
oldNullable
�� 
:
�� 
true
�� !
)
��! "
;
��" #
migrationBuilder
�� 
.
�� 
CreateTable
�� (
(
��( )
name
�� 
:
�� 
$str
�� $
,
��$ %
columns
�� 
:
�� 
table
�� 
=>
�� !
new
��" %
{
�� 
ID
�� 
=
�� 
table
�� 
.
�� 
Column
�� %
<
��% &
int
��& )
>
��) *
(
��* +
type
��+ /
:
��/ 0
$str
��1 6
,
��6 7
nullable
��8 @
:
��@ A
false
��B G
)
��G H
.
�� 

Annotation
�� #
(
��# $
$str
��$ 8
,
��8 9
$str
��: @
)
��@ A
,
��A B
BlogId
�� 
=
�� 
table
�� "
.
��" #
Column
��# )
<
��) *
int
��* -
>
��- .
(
��. /
type
��/ 3
:
��3 4
$str
��5 :
,
��: ;
nullable
��< D
:
��D E
true
��F J
)
��J K
,
��K L
CategoryName
��  
=
��! "
table
��# (
.
��( )
Column
��) /
<
��/ 0
string
��0 6
>
��6 7
(
��7 8
type
��8 <
:
��< =
$str
��> M
,
��M N
nullable
��O W
:
��W X
false
��Y ^
)
��^ _
}
�� 
,
�� 
constraints
�� 
:
�� 
table
�� "
=>
��# %
{
�� 
table
�� 
.
�� 

PrimaryKey
�� $
(
��$ %
$str
��% 6
,
��6 7
x
��8 9
=>
��: <
x
��= >
.
��> ?
ID
��? A
)
��A B
;
��B C
table
�� 
.
�� 

ForeignKey
�� $
(
��$ %
name
�� 
:
�� 
$str
�� <
,
��< =
column
�� 
:
�� 
x
��  !
=>
��" $
x
��% &
.
��& '
BlogId
��' -
,
��- .
principalTable
�� &
:
��& '
$str
��( /
,
��/ 0
principalColumn
�� '
:
��' (
$str
��) -
)
��- .
;
��. /
}
�� 
)
�� 
;
�� 
migrationBuilder
�� 
.
�� 
CreateTable
�� (
(
��( )
name
�� 
:
�� 
$str
�� !
,
��! "
columns
�� 
:
�� 
table
�� 
=>
�� !
new
��" %
{
�� 
ID
�� 
=
�� 
table
�� 
.
�� 
Column
�� %
<
��% &
int
��& )
>
��) *
(
��* +
type
��+ /
:
��/ 0
$str
��1 6
,
��6 7
nullable
��8 @
:
��@ A
false
��B G
)
��G H
.
�� 

Annotation
�� #
(
��# $
$str
��$ 8
,
��8 9
$str
��: @
)
��@ A
,
��A B
BlogId
�� 
=
�� 
table
�� "
.
��" #
Column
��# )
<
��) *
int
��* -
>
��- .
(
��. /
type
��/ 3
:
��3 4
$str
��5 :
,
��: ;
nullable
��< D
:
��D E
true
��F J
)
��J K
,
��K L
Url
�� 
=
�� 
table
�� 
.
��  
Column
��  &
<
��& '
string
��' -
>
��- .
(
��. /
type
��/ 3
:
��3 4
$str
��5 D
,
��D E
nullable
��F N
:
��N O
false
��P U
)
��U V
}
�� 
,
�� 
constraints
�� 
:
�� 
table
�� "
=>
��# %
{
�� 
table
�� 
.
�� 

PrimaryKey
�� $
(
��$ %
$str
��% 3
,
��3 4
x
��5 6
=>
��7 9
x
��: ;
.
��; <
ID
��< >
)
��> ?
;
��? @
table
�� 
.
�� 

ForeignKey
�� $
(
��$ %
name
�� 
:
�� 
$str
�� 9
,
��9 :
column
�� 
:
�� 
x
��  !
=>
��" $
x
��% &
.
��& '
BlogId
��' -
,
��- .
principalTable
�� &
:
��& '
$str
��( /
,
��/ 0
principalColumn
�� '
:
��' (
$str
��) -
)
��- .
;
��. /
}
�� 
)
�� 
;
�� 
migrationBuilder
�� 
.
�� 
CreateTable
�� (
(
��( )
name
�� 
:
�� 
$str
�� 
,
��  
columns
�� 
:
�� 
table
�� 
=>
�� !
new
��" %
{
�� 
ID
�� 
=
�� 
table
�� 
.
�� 
Column
�� %
<
��% &
int
��& )
>
��) *
(
��* +
type
��+ /
:
��/ 0
$str
��1 6
,
��6 7
nullable
��8 @
:
��@ A
false
��B G
)
��G H
.
�� 

Annotation
�� #
(
��# $
$str
��$ 8
,
��8 9
$str
��: @
)
��@ A
,
��A B
BlogId
�� 
=
�� 
table
�� "
.
��" #
Column
��# )
<
��) *
int
��* -
>
��- .
(
��. /
type
��/ 3
:
��3 4
$str
��5 :
,
��: ;
nullable
��< D
:
��D E
true
��F J
)
��J K
,
��K L
TagName
�� 
=
�� 
table
�� #
.
��# $
Column
��$ *
<
��* +
string
��+ 1
>
��1 2
(
��2 3
type
��3 7
:
��7 8
$str
��9 H
,
��H I
nullable
��J R
:
��R S
false
��T Y
)
��Y Z
}
�� 
,
�� 
constraints
�� 
:
�� 
table
�� "
=>
��# %
{
�� 
table
�� 
.
�� 

PrimaryKey
�� $
(
��$ %
$str
��% 1
,
��1 2
x
��3 4
=>
��5 7
x
��8 9
.
��9 :
ID
��: <
)
��< =
;
��= >
table
�� 
.
�� 

ForeignKey
�� $
(
��$ %
name
�� 
:
�� 
$str
�� 7
,
��7 8
column
�� 
:
�� 
x
��  !
=>
��" $
x
��% &
.
��& '
BlogId
��' -
,
��- .
principalTable
�� &
:
��& '
$str
��( /
,
��/ 0
principalColumn
�� '
:
��' (
$str
��) -
)
��- .
;
��. /
}
�� 
)
�� 
;
�� 
migrationBuilder
�� 
.
�� 
CreateTable
�� (
(
��( )
name
�� 
:
�� 
$str
�� 
,
�� 
columns
�� 
:
�� 
table
�� 
=>
�� !
new
��" %
{
�� 
UserId
�� 
=
�� 
table
�� "
.
��" #
Column
��# )
<
��) *
int
��* -
>
��- .
(
��. /
type
��/ 3
:
��3 4
$str
��5 :
,
��: ;
nullable
��< D
:
��D E
false
��F K
)
��K L
.
�� 

Annotation
�� #
(
��# $
$str
��$ 8
,
��8 9
$str
��: @
)
��@ A
,
��A B
Address
�� 
=
�� 
table
�� #
.
��# $
Column
��$ *
<
��* +
string
��+ 1
>
��1 2
(
��2 3
type
��3 7
:
��7 8
$str
��9 H
,
��H I
nullable
��J R
:
��R S
false
��T Y
)
��Y Z
,
��Z [
Avatar
�� 
=
�� 
table
�� "
.
��" #
Column
��# )
<
��) *
string
��* 0
>
��0 1
(
��1 2
type
��2 6
:
��6 7
$str
��8 G
,
��G H
nullable
��I Q
:
��Q R
false
��S X
)
��X Y
,
��Y Z
	CreatedAt
�� 
=
�� 
table
��  %
.
��% &
Column
��& ,
<
��, -
DateTime
��- 5
>
��5 6
(
��6 7
type
��7 ;
:
��; <
$str
��= H
,
��H I
nullable
��J R
:
��R S
false
��T Y
)
��Y Z
,
��Z [
DateOfBirth
�� 
=
��  !
table
��" '
.
��' (
Column
��( .
<
��. /
DateTime
��/ 7
>
��7 8
(
��8 9
type
��9 =
:
��= >
$str
��? J
,
��J K
nullable
��L T
:
��T U
false
��V [
)
��[ \
,
��\ ]
Email
�� 
=
�� 
table
�� !
.
��! "
Column
��" (
<
��( )
string
��) /
>
��/ 0
(
��0 1
type
��1 5
:
��5 6
$str
��7 F
,
��F G
nullable
��H P
:
��P Q
false
��R W
)
��W X
,
��X Y
FullName
�� 
=
�� 
table
�� $
.
��$ %
Column
��% +
<
��+ ,
string
��, 2
>
��2 3
(
��3 4
type
��4 8
:
��8 9
$str
��: I
,
��I J
nullable
��K S
:
��S T
false
��U Z
)
��Z [
,
��[ \
LastLoginAt
�� 
=
��  !
table
��" '
.
��' (
Column
��( .
<
��. /
DateTime
��/ 7
>
��7 8
(
��8 9
type
��9 =
:
��= >
$str
��? J
,
��J K
nullable
��L T
:
��T U
true
��V Z
)
��Z [
,
��[ \
LastSolvedAt
��  
=
��! "
table
��# (
.
��( )
Column
��) /
<
��/ 0
DateTime
��0 8
>
��8 9
(
��9 :
type
��: >
:
��> ?
$str
��@ K
,
��K L
nullable
��M U
:
��U V
true
��W [
)
��[ \
,
��\ ]
Password
�� 
=
�� 
table
�� $
.
��$ %
Column
��% +
<
��+ ,
string
��, 2
>
��2 3
(
��3 4
type
��4 8
:
��8 9
$str
��: I
,
��I J
nullable
��K S
:
��S T
false
��U Z
)
��Z [
,
��[ \
Phone
�� 
=
�� 
table
�� !
.
��! "
Column
��" (
<
��( )
string
��) /
>
��/ 0
(
��0 1
type
��1 5
:
��5 6
$str
��7 F
,
��F G
nullable
��H P
:
��P Q
false
��R W
)
��W X
,
��X Y
TotalSolved
�� 
=
��  !
table
��" '
.
��' (
Column
��( .
<
��. /
int
��/ 2
>
��2 3
(
��3 4
type
��4 8
:
��8 9
$str
��: ?
,
��? @
nullable
��A I
:
��I J
false
��K P
)
��P Q
,
��Q R
TotalSubmissions
�� $
=
��% &
table
��' ,
.
��, -
Column
��- 3
<
��3 4
int
��4 7
>
��7 8
(
��8 9
type
��9 =
:
��= >
$str
��? D
,
��D E
nullable
��F N
:
��N O
false
��P U
)
��U V
,
��V W
Username
�� 
=
�� 
table
�� $
.
��$ %
Column
��% +
<
��+ ,
string
��, 2
>
��2 3
(
��3 4
type
��4 8
:
��8 9
$str
��: I
,
��I J
nullable
��K S
:
��S T
false
��U Z
)
��Z [
}
�� 
,
�� 
constraints
�� 
:
�� 
table
�� "
=>
��# %
{
�� 
table
�� 
.
�� 

PrimaryKey
�� $
(
��$ %
$str
��% .
,
��. /
x
��0 1
=>
��2 4
x
��5 6
.
��6 7
UserId
��7 =
)
��= >
;
��> ?
}
�� 
)
�� 
;
�� 
migrationBuilder
�� 
.
�� 

InsertData
�� '
(
��' (
table
�� 
:
�� 
$str
�� $
,
��$ %
columns
�� 
:
�� 
new
�� 
[
�� 
]
�� 
{
��  
$str
��! %
,
��% &
$str
��' 9
,
��9 :
$str
��; A
,
��A B
$str
��C S
}
��T U
,
��U V
values
�� 
:
�� 
new
�� 
object
�� "
[
��" #
,
��# $
]
��$ %
{
�� 
{
�� 
$str
�� <
,
��< =
null
��> B
,
��B C
$str
��D K
,
��K L
$str
��M T
}
��U V
,
��V W
{
�� 
$str
�� <
,
��< =
null
��> B
,
��B C
$str
��D K
,
��K L
$str
��M T
}
��U V
,
��V W
{
�� 
$str
�� <
,
��< =
null
��> B
,
��B C
$str
��D J
,
��J K
$str
��L R
}
��S T
}
�� 
)
�� 
;
�� 
migrationBuilder
�� 
.
�� 
CreateIndex
�� (
(
��( )
name
�� 
:
�� 
$str
�� .
,
��. /
table
�� 
:
�� 
$str
�� %
,
��% &
column
�� 
:
�� 
$str
��  
)
��  !
;
��! "
migrationBuilder
�� 
.
�� 
CreateIndex
�� (
(
��( )
name
�� 
:
�� 
$str
�� +
,
��+ ,
table
�� 
:
�� 
$str
�� "
,
��" #
column
�� 
:
�� 
$str
��  
)
��  !
;
��! "
migrationBuilder
�� 
.
�� 
CreateIndex
�� (
(
��( )
name
�� 
:
�� 
$str
�� )
,
��) *
table
�� 
:
�� 
$str
��  
,
��  !
column
�� 
:
�� 
$str
��  
)
��  !
;
��! "
migrationBuilder
�� 
.
�� 
AddForeignKey
�� *
(
��* +
name
�� 
:
�� 
$str
�� ,
,
��, -
table
�� 
:
�� 
$str
�� 
,
�� 
column
�� 
:
�� 
$str
��  
,
��  !
principalTable
�� 
:
�� 
$str
��  &
,
��& '
principalColumn
�� 
:
��  
$str
��! )
)
��) *
;
��* +
}
�� 	
}
�� 
}�� �$
eD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Migrations\20250206110549_AddNewEntity.cs
	namespace 	
api
 
. 

Migrations 
{ 
public

 

partial

 
class

 
AddNewEntity

 %
:

& '
	Migration

( 1
{ 
	protected 
override 
void 
Up  "
(" #
MigrationBuilder# 3
migrationBuilder4 D
)D E
{ 	
migrationBuilder 
. 

DeleteData '
(' (
table 
: 
$str $
,$ %
	keyColumn 
: 
$str 
,  
keyValue 
: 
$str @
)@ A
;A B
migrationBuilder 
. 

DeleteData '
(' (
table 
: 
$str $
,$ %
	keyColumn 
: 
$str 
,  
keyValue 
: 
$str @
)@ A
;A B
migrationBuilder 
. 

DeleteData '
(' (
table 
: 
$str $
,$ %
	keyColumn 
: 
$str 
,  
keyValue 
: 
$str @
)@ A
;A B
migrationBuilder 
. 

InsertData '
(' (
table 
: 
$str $
,$ %
columns   
:   
new   
[   
]   
{    
$str  ! %
,  % &
$str  ' 9
,  9 :
$str  ; A
,  A B
$str  C S
}  T U
,  U V
values!! 
:!! 
new!! 
object!! "
[!!" #
,!!# $
]!!$ %
{"" 
{## 
$str## <
,##< =
null##> B
,##B C
$str##D K
,##K L
$str##M T
}##U V
,##V W
{$$ 
$str$$ <
,$$< =
null$$> B
,$$B C
$str$$D K
,$$K L
$str$$M T
}$$U V
,$$V W
{%% 
$str%% <
,%%< =
null%%> B
,%%B C
$str%%D J
,%%J K
$str%%L R
}%%S T
}&& 
)&& 
;&& 
}'' 	
	protected** 
override** 
void** 
Down**  $
(**$ %
MigrationBuilder**% 5
migrationBuilder**6 F
)**F G
{++ 	
migrationBuilder,, 
.,, 

DeleteData,, '
(,,' (
table-- 
:-- 
$str-- $
,--$ %
	keyColumn.. 
:.. 
$str.. 
,..  
keyValue// 
:// 
$str// @
)//@ A
;//A B
migrationBuilder11 
.11 

DeleteData11 '
(11' (
table22 
:22 
$str22 $
,22$ %
	keyColumn33 
:33 
$str33 
,33  
keyValue44 
:44 
$str44 @
)44@ A
;44A B
migrationBuilder66 
.66 

DeleteData66 '
(66' (
table77 
:77 
$str77 $
,77$ %
	keyColumn88 
:88 
$str88 
,88  
keyValue99 
:99 
$str99 @
)99@ A
;99A B
migrationBuilder;; 
.;; 

InsertData;; '
(;;' (
table<< 
:<< 
$str<< $
,<<$ %
columns== 
:== 
new== 
[== 
]== 
{==  
$str==! %
,==% &
$str==' 9
,==9 :
$str==; A
,==A B
$str==C S
}==T U
,==U V
values>> 
:>> 
new>> 
object>> "
[>>" #
,>># $
]>>$ %
{?? 
{@@ 
$str@@ <
,@@< =
null@@> B
,@@B C
$str@@D K
,@@K L
$str@@M T
}@@U V
,@@V W
{AA 
$strAA <
,AA< =
nullAA> B
,AAB C
$strAAD J
,AAJ K
$strAAL R
}AAS T
,AAT U
{BB 
$strBB <
,BB< =
nullBB> B
,BBB C
$strBBD K
,BBK L
$strBBM T
}BBU V
}CC 
)CC 
;CC 
}DD 	
}EE 
}FF �$
bD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Migrations\20250206095430_newEntity.cs
	namespace 	
api
 
. 

Migrations 
{ 
public

 

partial

 
class

 
	newEntity

 "
:

# $
	Migration

% .
{ 
	protected 
override 
void 
Up  "
(" #
MigrationBuilder# 3
migrationBuilder4 D
)D E
{ 	
migrationBuilder 
. 

DeleteData '
(' (
table 
: 
$str $
,$ %
	keyColumn 
: 
$str 
,  
keyValue 
: 
$str @
)@ A
;A B
migrationBuilder 
. 

DeleteData '
(' (
table 
: 
$str $
,$ %
	keyColumn 
: 
$str 
,  
keyValue 
: 
$str @
)@ A
;A B
migrationBuilder 
. 

DeleteData '
(' (
table 
: 
$str $
,$ %
	keyColumn 
: 
$str 
,  
keyValue 
: 
$str @
)@ A
;A B
migrationBuilder 
. 

InsertData '
(' (
table 
: 
$str $
,$ %
columns   
:   
new   
[   
]   
{    
$str  ! %
,  % &
$str  ' 9
,  9 :
$str  ; A
,  A B
$str  C S
}  T U
,  U V
values!! 
:!! 
new!! 
object!! "
[!!" #
,!!# $
]!!$ %
{"" 
{## 
$str## <
,##< =
null##> B
,##B C
$str##D K
,##K L
$str##M T
}##U V
,##V W
{$$ 
$str$$ <
,$$< =
null$$> B
,$$B C
$str$$D J
,$$J K
$str$$L R
}$$S T
,$$T U
{%% 
$str%% <
,%%< =
null%%> B
,%%B C
$str%%D K
,%%K L
$str%%M T
}%%U V
}&& 
)&& 
;&& 
}'' 	
	protected** 
override** 
void** 
Down**  $
(**$ %
MigrationBuilder**% 5
migrationBuilder**6 F
)**F G
{++ 	
migrationBuilder,, 
.,, 

DeleteData,, '
(,,' (
table-- 
:-- 
$str-- $
,--$ %
	keyColumn.. 
:.. 
$str.. 
,..  
keyValue// 
:// 
$str// @
)//@ A
;//A B
migrationBuilder11 
.11 

DeleteData11 '
(11' (
table22 
:22 
$str22 $
,22$ %
	keyColumn33 
:33 
$str33 
,33  
keyValue44 
:44 
$str44 @
)44@ A
;44A B
migrationBuilder66 
.66 

DeleteData66 '
(66' (
table77 
:77 
$str77 $
,77$ %
	keyColumn88 
:88 
$str88 
,88  
keyValue99 
:99 
$str99 @
)99@ A
;99A B
migrationBuilder;; 
.;; 

InsertData;; '
(;;' (
table<< 
:<< 
$str<< $
,<<$ %
columns== 
:== 
new== 
[== 
]== 
{==  
$str==! %
,==% &
$str==' 9
,==9 :
$str==; A
,==A B
$str==C S
}==T U
,==U V
values>> 
:>> 
new>> 
object>> "
[>>" #
,>># $
]>>$ %
{?? 
{@@ 
$str@@ <
,@@< =
null@@> B
,@@B C
$str@@D K
,@@K L
$str@@M T
}@@U V
,@@V W
{AA 
$strAA <
,AA< =
nullAA> B
,AAB C
$strAAD K
,AAK L
$strAAM T
}AAU V
,AAV W
{BB 
$strBB <
,BB< =
nullBB> B
,BBB C
$strBBD J
,BBJ K
$strBBL R
}BBS T
}CC 
)CC 
;CC 
}DD 	
}EE 
}FF ��
]D:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Migrations\20250205074810_Init.cs
	namespace 	
api
 
. 

Migrations 
{		 
public 

partial 
class 
Init 
: 
	Migration  )
{ 
	protected 
override 
void 
Up  "
(" #
MigrationBuilder# 3
migrationBuilder4 D
)D E
{ 	
migrationBuilder 
. 
CreateTable (
(( )
name 
: 
$str #
,# $
columns 
: 
table 
=> !
new" %
{ 
Id 
= 
table 
. 
Column %
<% &
string& ,
>, -
(- .
type. 2
:2 3
$str4 C
,C D
nullableE M
:M N
falseO T
)T U
,U V
Name 
= 
table  
.  !
Column! '
<' (
string( .
>. /
(/ 0
type0 4
:4 5
$str6 E
,E F
	maxLengthG P
:P Q
$numR U
,U V
nullableW _
:_ `
truea e
)e f
,f g
NormalizedName "
=# $
table% *
.* +
Column+ 1
<1 2
string2 8
>8 9
(9 :
type: >
:> ?
$str@ O
,O P
	maxLengthQ Z
:Z [
$num\ _
,_ `
nullablea i
:i j
truek o
)o p
,p q
ConcurrencyStamp $
=% &
table' ,
., -
Column- 3
<3 4
string4 :
>: ;
(; <
type< @
:@ A
$strB Q
,Q R
nullableS [
:[ \
true] a
)a b
} 
, 
constraints 
: 
table "
=># %
{ 
table 
. 

PrimaryKey $
($ %
$str% 5
,5 6
x7 8
=>9 ;
x< =
.= >
Id> @
)@ A
;A B
} 
) 
; 
migrationBuilder 
. 
CreateTable (
(( )
name 
: 
$str #
,# $
columns   
:   
table   
=>   !
new  " %
{!! 
Id"" 
="" 
table"" 
."" 
Column"" %
<""% &
string""& ,
>"", -
(""- .
type"". 2
:""2 3
$str""4 C
,""C D
nullable""E M
:""M N
false""O T
)""T U
,""U V
FullName## 
=## 
table## $
.##$ %
Column##% +
<##+ ,
string##, 2
>##2 3
(##3 4
type##4 8
:##8 9
$str##: I
,##I J
nullable##K S
:##S T
true##U Y
)##Y Z
,##Z [
Address$$ 
=$$ 
table$$ #
.$$# $
Column$$$ *
<$$* +
string$$+ 1
>$$1 2
($$2 3
type$$3 7
:$$7 8
$str$$9 H
,$$H I
nullable$$J R
:$$R S
true$$T X
)$$X Y
,$$Y Z
Avatar%% 
=%% 
table%% "
.%%" #
Column%%# )
<%%) *
string%%* 0
>%%0 1
(%%1 2
type%%2 6
:%%6 7
$str%%8 G
,%%G H
nullable%%I Q
:%%Q R
true%%S W
)%%W X
,%%X Y
DateOfBirth&& 
=&&  !
table&&" '
.&&' (
Column&&( .
<&&. /
DateTime&&/ 7
>&&7 8
(&&8 9
type&&9 =
:&&= >
$str&&? J
,&&J K
nullable&&L T
:&&T U
true&&V Z
)&&Z [
,&&[ \
PhoneNumber'' 
=''  !
table''" '
.''' (
Column''( .
<''. /
string''/ 5
>''5 6
(''6 7
type''7 ;
:''; <
$str''= L
,''L M
nullable''N V
:''V W
true''X \
)''\ ]
,''] ^
	CreatedAt(( 
=(( 
table((  %
.((% &
Column((& ,
<((, -
DateTime((- 5
>((5 6
(((6 7
type((7 ;
:((; <
$str((= H
,((H I
nullable((J R
:((R S
false((T Y
)((Y Z
,((Z [
LastLoginAt)) 
=))  !
table))" '
.))' (
Column))( .
<)). /
DateTime))/ 7
>))7 8
())8 9
type))9 =
:))= >
$str))? J
,))J K
nullable))L T
:))T U
true))V Z
)))Z [
,))[ \
TotalSolved** 
=**  !
table**" '
.**' (
Column**( .
<**. /
int**/ 2
>**2 3
(**3 4
type**4 8
:**8 9
$str**: ?
,**? @
nullable**A I
:**I J
false**K P
)**P Q
,**Q R
TotalSubmissions++ $
=++% &
table++' ,
.++, -
Column++- 3
<++3 4
int++4 7
>++7 8
(++8 9
type++9 =
:++= >
$str++? D
,++D E
nullable++F N
:++N O
false++P U
)++U V
,++V W
LastSolvedAt,,  
=,,! "
table,,# (
.,,( )
Column,,) /
<,,/ 0
DateTime,,0 8
>,,8 9
(,,9 :
type,,: >
:,,> ?
$str,,@ K
,,,K L
nullable,,M U
:,,U V
true,,W [
),,[ \
,,,\ ]
UserName-- 
=-- 
table-- $
.--$ %
Column--% +
<--+ ,
string--, 2
>--2 3
(--3 4
type--4 8
:--8 9
$str--: I
,--I J
	maxLength--K T
:--T U
$num--V Y
,--Y Z
nullable--[ c
:--c d
true--e i
)--i j
,--j k
NormalizedUserName.. &
=..' (
table..) .
.... /
Column../ 5
<..5 6
string..6 <
>..< =
(..= >
type..> B
:..B C
$str..D S
,..S T
	maxLength..U ^
:..^ _
$num..` c
,..c d
nullable..e m
:..m n
true..o s
)..s t
,..t u
Email// 
=// 
table// !
.//! "
Column//" (
<//( )
string//) /
>/// 0
(//0 1
type//1 5
://5 6
$str//7 F
,//F G
	maxLength//H Q
://Q R
$num//S V
,//V W
nullable//X `
://` a
true//b f
)//f g
,//g h
NormalizedEmail00 #
=00$ %
table00& +
.00+ ,
Column00, 2
<002 3
string003 9
>009 :
(00: ;
type00; ?
:00? @
$str00A P
,00P Q
	maxLength00R [
:00[ \
$num00] `
,00` a
nullable00b j
:00j k
true00l p
)00p q
,00q r
EmailConfirmed11 "
=11# $
table11% *
.11* +
Column11+ 1
<111 2
bool112 6
>116 7
(117 8
type118 <
:11< =
$str11> C
,11C D
nullable11E M
:11M N
false11O T
)11T U
,11U V
PasswordHash22  
=22! "
table22# (
.22( )
Column22) /
<22/ 0
string220 6
>226 7
(227 8
type228 <
:22< =
$str22> M
,22M N
nullable22O W
:22W X
true22Y ]
)22] ^
,22^ _
SecurityStamp33 !
=33" #
table33$ )
.33) *
Column33* 0
<330 1
string331 7
>337 8
(338 9
type339 =
:33= >
$str33? N
,33N O
nullable33P X
:33X Y
true33Z ^
)33^ _
,33_ `
ConcurrencyStamp44 $
=44% &
table44' ,
.44, -
Column44- 3
<443 4
string444 :
>44: ;
(44; <
type44< @
:44@ A
$str44B Q
,44Q R
nullable44S [
:44[ \
true44] a
)44a b
,44b c 
PhoneNumberConfirmed55 (
=55) *
table55+ 0
.550 1
Column551 7
<557 8
bool558 <
>55< =
(55= >
type55> B
:55B C
$str55D I
,55I J
nullable55K S
:55S T
false55U Z
)55Z [
,55[ \
TwoFactorEnabled66 $
=66% &
table66' ,
.66, -
Column66- 3
<663 4
bool664 8
>668 9
(669 :
type66: >
:66> ?
$str66@ E
,66E F
nullable66G O
:66O P
false66Q V
)66V W
,66W X

LockoutEnd77 
=77  
table77! &
.77& '
Column77' -
<77- .
DateTimeOffset77. <
>77< =
(77= >
type77> B
:77B C
$str77D T
,77T U
nullable77V ^
:77^ _
true77` d
)77d e
,77e f
LockoutEnabled88 "
=88# $
table88% *
.88* +
Column88+ 1
<881 2
bool882 6
>886 7
(887 8
type888 <
:88< =
$str88> C
,88C D
nullable88E M
:88M N
false88O T
)88T U
,88U V
AccessFailedCount99 %
=99& '
table99( -
.99- .
Column99. 4
<994 5
int995 8
>998 9
(999 :
type99: >
:99> ?
$str99@ E
,99E F
nullable99G O
:99O P
false99Q V
)99V W
}:: 
,:: 
constraints;; 
:;; 
table;; "
=>;;# %
{<< 
table== 
.== 

PrimaryKey== $
(==$ %
$str==% 5
,==5 6
x==7 8
=>==9 ;
x==< =
.=== >
Id==> @
)==@ A
;==A B
}>> 
)>> 
;>> 
migrationBuilder@@ 
.@@ 
CreateTable@@ (
(@@( )
nameAA 
:AA 
$strAA  
,AA  !
columnsBB 
:BB 
tableBB 
=>BB !
newBB" %
{CC 
	ContestIdDD 
=DD 
tableDD  %
.DD% &
ColumnDD& ,
<DD, -
stringDD- 3
>DD3 4
(DD4 5
typeDD5 9
:DD9 :
$strDD; J
,DDJ K
nullableDDL T
:DDT U
falseDDV [
)DD[ \
,DD\ ]
ContestNameEE 
=EE  !
tableEE" '
.EE' (
ColumnEE( .
<EE. /
stringEE/ 5
>EE5 6
(EE6 7
typeEE7 ;
:EE; <
$strEE= L
,EEL M
	maxLengthEEN W
:EEW X
$numEEY \
,EE\ ]
nullableEE^ f
:EEf g
falseEEh m
)EEm n
,EEn o
DueTimeFF 
=FF 
tableFF #
.FF# $
ColumnFF$ *
<FF* +
DateTimeFF+ 3
>FF3 4
(FF4 5
typeFF5 9
:FF9 :
$strFF; F
,FFF G
nullableFFH P
:FFP Q
falseFFR W
)FFW X
,FFX Y
	CreatedAtGG 
=GG 
tableGG  %
.GG% &
ColumnGG& ,
<GG, -
DateTimeGG- 5
>GG5 6
(GG6 7
typeGG7 ;
:GG; <
$strGG= H
,GGH I
nullableGGJ R
:GGR S
falseGGT Y
)GGY Z
,GGZ [

TotalPointHH 
=HH  
tableHH! &
.HH& '
ColumnHH' -
<HH- .
intHH. 1
>HH1 2
(HH2 3
typeHH3 7
:HH7 8
$strHH9 >
,HH> ?
nullableHH@ H
:HHH I
falseHHJ O
)HHO P
,HHP Q
LevelII 
=II 
tableII !
.II! "
ColumnII" (
<II( )
intII) ,
>II, -
(II- .
typeII. 2
:II2 3
$strII4 9
,II9 :
nullableII; C
:IIC D
falseIIE J
)IIJ K
,IIK L
EndDateJJ 
=JJ 
tableJJ #
.JJ# $
ColumnJJ$ *
<JJ* +
DateTimeJJ+ 3
>JJ3 4
(JJ4 5
typeJJ5 9
:JJ9 :
$strJJ; F
,JJF G
nullableJJH P
:JJP Q
falseJJR W
)JJW X
}KK 
,KK 
constraintsLL 
:LL 
tableLL "
=>LL# %
{MM 
tableNN 
.NN 

PrimaryKeyNN $
(NN$ %
$strNN% 2
,NN2 3
xNN4 5
=>NN6 8
xNN9 :
.NN: ;
	ContestIdNN; D
)NND E
;NNE F
}OO 
)OO 
;OO 
migrationBuilderQQ 
.QQ 
CreateTableQQ (
(QQ( )
nameRR 
:RR 
$strRR  
,RR  !
columnsSS 
:SS 
tableSS 
=>SS !
newSS" %
{TT 
	ProblemIdUU 
=UU 
tableUU  %
.UU% &
ColumnUU& ,
<UU, -
stringUU- 3
>UU3 4
(UU4 5
typeUU5 9
:UU9 :
$strUU; J
,UUJ K
nullableUUL T
:UUT U
falseUUV [
)UU[ \
,UU\ ]
CategoryVV 
=VV 
tableVV $
.VV$ %
ColumnVV% +
<VV+ ,
stringVV, 2
>VV2 3
(VV3 4
typeVV4 8
:VV8 9
$strVV: I
,VVI J
	maxLengthVVK T
:VVT U
$numVVV Y
,VVY Z
nullableVV[ c
:VVc d
falseVVe j
)VVj k
,VVk l
TitleWW 
=WW 
tableWW !
.WW! "
ColumnWW" (
<WW( )
stringWW) /
>WW/ 0
(WW0 1
typeWW1 5
:WW5 6
$strWW7 F
,WWF G
	maxLengthWWH Q
:WWQ R
$numWWS V
,WWV W
nullableWWX `
:WW` a
falseWWb g
)WWg h
,WWh i
DetailXX 
=XX 
tableXX "
.XX" #
ColumnXX# )
<XX) *
stringXX* 0
>XX0 1
(XX1 2
typeXX2 6
:XX6 7
$strXX8 G
,XXG H
nullableXXI Q
:XXQ R
falseXXS X
)XXX Y
,XXY Z
InputYY 
=YY 
tableYY !
.YY! "
ColumnYY" (
<YY( )
stringYY) /
>YY/ 0
(YY0 1
typeYY1 5
:YY5 6
$strYY7 F
,YYF G
nullableYYH P
:YYP Q
falseYYR W
)YYW X
,YYX Y
OutputZZ 
=ZZ 
tableZZ "
.ZZ" #
ColumnZZ# )
<ZZ) *
stringZZ* 0
>ZZ0 1
(ZZ1 2
typeZZ2 6
:ZZ6 7
$strZZ8 G
,ZZG H
nullableZZI Q
:ZZQ R
falseZZS X
)ZZX Y
,ZZY Z

TotalPoint[[ 
=[[  
table[[! &
.[[& '
Column[[' -
<[[- .
int[[. 1
>[[1 2
([[2 3
type[[3 7
:[[7 8
$str[[9 >
,[[> ?
nullable[[@ H
:[[H I
false[[J O
)[[O P
,[[P Q
	TimeLimit\\ 
=\\ 
table\\  %
.\\% &
Column\\& ,
<\\, -
int\\- 0
>\\0 1
(\\1 2
type\\2 6
:\\6 7
$str\\8 =
,\\= >
nullable\\? G
:\\G H
false\\I N
)\\N O
,\\O P
MemoryLimit]] 
=]]  !
table]]" '
.]]' (
Column]]( .
<]]. /
int]]/ 2
>]]2 3
(]]3 4
type]]4 8
:]]8 9
$str]]: ?
,]]? @
nullable]]A I
:]]I J
false]]K P
)]]P Q
,]]Q R
Author^^ 
=^^ 
table^^ "
.^^" #
Column^^# )
<^^) *
string^^* 0
>^^0 1
(^^1 2
type^^2 6
:^^6 7
$str^^8 G
,^^G H
	maxLength^^I R
:^^R S
$num^^T W
,^^W X
nullable^^Y a
:^^a b
true^^c g
)^^g h
,^^h i
Solution__ 
=__ 
table__ $
.__$ %
Column__% +
<__+ ,
string__, 2
>__2 3
(__3 4
type__4 8
:__8 9
$str__: I
,__I J
nullable__K S
:__S T
true__U Y
)__Y Z
}`` 
,`` 
constraintsaa 
:aa 
tableaa "
=>aa# %
{bb 
tablecc 
.cc 

PrimaryKeycc $
(cc$ %
$strcc% 2
,cc2 3
xcc4 5
=>cc6 8
xcc9 :
.cc: ;
	ProblemIdcc; D
)ccD E
;ccE F
}dd 
)dd 
;dd 
migrationBuilderff 
.ff 
CreateTableff (
(ff( )
namegg 
:gg 
$strgg (
,gg( )
columnshh 
:hh 
tablehh 
=>hh !
newhh" %
{ii 
Idjj 
=jj 
tablejj 
.jj 
Columnjj %
<jj% &
intjj& )
>jj) *
(jj* +
typejj+ /
:jj/ 0
$strjj1 6
,jj6 7
nullablejj8 @
:jj@ A
falsejjB G
)jjG H
.kk 

Annotationkk #
(kk# $
$strkk$ 8
,kk8 9
$strkk: @
)kk@ A
,kkA B
RoleIdll 
=ll 
tablell "
.ll" #
Columnll# )
<ll) *
stringll* 0
>ll0 1
(ll1 2
typell2 6
:ll6 7
$strll8 G
,llG H
nullablellI Q
:llQ R
falsellS X
)llX Y
,llY Z
	ClaimTypemm 
=mm 
tablemm  %
.mm% &
Columnmm& ,
<mm, -
stringmm- 3
>mm3 4
(mm4 5
typemm5 9
:mm9 :
$strmm; J
,mmJ K
nullablemmL T
:mmT U
truemmV Z
)mmZ [
,mm[ \

ClaimValuenn 
=nn  
tablenn! &
.nn& '
Columnnn' -
<nn- .
stringnn. 4
>nn4 5
(nn5 6
typenn6 :
:nn: ;
$strnn< K
,nnK L
nullablennM U
:nnU V
truennW [
)nn[ \
}oo 
,oo 
constraintspp 
:pp 
tablepp "
=>pp# %
{qq 
tablerr 
.rr 

PrimaryKeyrr $
(rr$ %
$strrr% :
,rr: ;
xrr< =
=>rr> @
xrrA B
.rrB C
IdrrC E
)rrE F
;rrF G
tabless 
.ss 

ForeignKeyss $
(ss$ %
namett 
:tt 
$strtt F
,ttF G
columnuu 
:uu 
xuu  !
=>uu" $
xuu% &
.uu& '
RoleIduu' -
,uu- .
principalTablevv &
:vv& '
$strvv( 5
,vv5 6
principalColumnww '
:ww' (
$strww) -
,ww- .
onDeletexx  
:xx  !
ReferentialActionxx" 3
.xx3 4
Cascadexx4 ;
)xx; <
;xx< =
}yy 
)yy 
;yy 
migrationBuilder{{ 
.{{ 
CreateTable{{ (
({{( )
name|| 
:|| 
$str|| (
,||( )
columns}} 
:}} 
table}} 
=>}} !
new}}" %
{~~ 
Id 
= 
table 
. 
Column %
<% &
int& )
>) *
(* +
type+ /
:/ 0
$str1 6
,6 7
nullable8 @
:@ A
falseB G
)G H
.
�� 

Annotation
�� #
(
��# $
$str
��$ 8
,
��8 9
$str
��: @
)
��@ A
,
��A B
UserId
�� 
=
�� 
table
�� "
.
��" #
Column
��# )
<
��) *
string
��* 0
>
��0 1
(
��1 2
type
��2 6
:
��6 7
$str
��8 G
,
��G H
nullable
��I Q
:
��Q R
false
��S X
)
��X Y
,
��Y Z
	ClaimType
�� 
=
�� 
table
��  %
.
��% &
Column
��& ,
<
��, -
string
��- 3
>
��3 4
(
��4 5
type
��5 9
:
��9 :
$str
��; J
,
��J K
nullable
��L T
:
��T U
true
��V Z
)
��Z [
,
��[ \

ClaimValue
�� 
=
��  
table
��! &
.
��& '
Column
��' -
<
��- .
string
��. 4
>
��4 5
(
��5 6
type
��6 :
:
��: ;
$str
��< K
,
��K L
nullable
��M U
:
��U V
true
��W [
)
��[ \
}
�� 
,
�� 
constraints
�� 
:
�� 
table
�� "
=>
��# %
{
�� 
table
�� 
.
�� 

PrimaryKey
�� $
(
��$ %
$str
��% :
,
��: ;
x
��< =
=>
��> @
x
��A B
.
��B C
Id
��C E
)
��E F
;
��F G
table
�� 
.
�� 

ForeignKey
�� $
(
��$ %
name
�� 
:
�� 
$str
�� F
,
��F G
column
�� 
:
�� 
x
��  !
=>
��" $
x
��% &
.
��& '
UserId
��' -
,
��- .
principalTable
�� &
:
��& '
$str
��( 5
,
��5 6
principalColumn
�� '
:
��' (
$str
��) -
,
��- .
onDelete
��  
:
��  !
ReferentialAction
��" 3
.
��3 4
Cascade
��4 ;
)
��; <
;
��< =
}
�� 
)
�� 
;
�� 
migrationBuilder
�� 
.
�� 
CreateTable
�� (
(
��( )
name
�� 
:
�� 
$str
�� (
,
��( )
columns
�� 
:
�� 
table
�� 
=>
�� !
new
��" %
{
�� 
LoginProvider
�� !
=
��" #
table
��$ )
.
��) *
Column
��* 0
<
��0 1
string
��1 7
>
��7 8
(
��8 9
type
��9 =
:
��= >
$str
��? N
,
��N O
nullable
��P X
:
��X Y
false
��Z _
)
��_ `
,
��` a
ProviderKey
�� 
=
��  !
table
��" '
.
��' (
Column
��( .
<
��. /
string
��/ 5
>
��5 6
(
��6 7
type
��7 ;
:
��; <
$str
��= L
,
��L M
nullable
��N V
:
��V W
false
��X ]
)
��] ^
,
��^ _!
ProviderDisplayName
�� '
=
��( )
table
��* /
.
��/ 0
Column
��0 6
<
��6 7
string
��7 =
>
��= >
(
��> ?
type
��? C
:
��C D
$str
��E T
,
��T U
nullable
��V ^
:
��^ _
true
��` d
)
��d e
,
��e f
UserId
�� 
=
�� 
table
�� "
.
��" #
Column
��# )
<
��) *
string
��* 0
>
��0 1
(
��1 2
type
��2 6
:
��6 7
$str
��8 G
,
��G H
nullable
��I Q
:
��Q R
false
��S X
)
��X Y
}
�� 
,
�� 
constraints
�� 
:
�� 
table
�� "
=>
��# %
{
�� 
table
�� 
.
�� 

PrimaryKey
�� $
(
��$ %
$str
��% :
,
��: ;
x
��< =
=>
��> @
new
��A D
{
��E F
x
��G H
.
��H I
LoginProvider
��I V
,
��V W
x
��X Y
.
��Y Z
ProviderKey
��Z e
}
��f g
)
��g h
;
��h i
table
�� 
.
�� 

ForeignKey
�� $
(
��$ %
name
�� 
:
�� 
$str
�� F
,
��F G
column
�� 
:
�� 
x
��  !
=>
��" $
x
��% &
.
��& '
UserId
��' -
,
��- .
principalTable
�� &
:
��& '
$str
��( 5
,
��5 6
principalColumn
�� '
:
��' (
$str
��) -
,
��- .
onDelete
��  
:
��  !
ReferentialAction
��" 3
.
��3 4
Cascade
��4 ;
)
��; <
;
��< =
}
�� 
)
�� 
;
�� 
migrationBuilder
�� 
.
�� 
CreateTable
�� (
(
��( )
name
�� 
:
�� 
$str
�� '
,
��' (
columns
�� 
:
�� 
table
�� 
=>
�� !
new
��" %
{
�� 
UserId
�� 
=
�� 
table
�� "
.
��" #
Column
��# )
<
��) *
string
��* 0
>
��0 1
(
��1 2
type
��2 6
:
��6 7
$str
��8 G
,
��G H
nullable
��I Q
:
��Q R
false
��S X
)
��X Y
,
��Y Z
RoleId
�� 
=
�� 
table
�� "
.
��" #
Column
��# )
<
��) *
string
��* 0
>
��0 1
(
��1 2
type
��2 6
:
��6 7
$str
��8 G
,
��G H
nullable
��I Q
:
��Q R
false
��S X
)
��X Y
}
�� 
,
�� 
constraints
�� 
:
�� 
table
�� "
=>
��# %
{
�� 
table
�� 
.
�� 

PrimaryKey
�� $
(
��$ %
$str
��% 9
,
��9 :
x
��; <
=>
��= ?
new
��@ C
{
��D E
x
��F G
.
��G H
UserId
��H N
,
��N O
x
��P Q
.
��Q R
RoleId
��R X
}
��Y Z
)
��Z [
;
��[ \
table
�� 
.
�� 

ForeignKey
�� $
(
��$ %
name
�� 
:
�� 
$str
�� E
,
��E F
column
�� 
:
�� 
x
��  !
=>
��" $
x
��% &
.
��& '
RoleId
��' -
,
��- .
principalTable
�� &
:
��& '
$str
��( 5
,
��5 6
principalColumn
�� '
:
��' (
$str
��) -
,
��- .
onDelete
��  
:
��  !
ReferentialAction
��" 3
.
��3 4
Cascade
��4 ;
)
��; <
;
��< =
table
�� 
.
�� 

ForeignKey
�� $
(
��$ %
name
�� 
:
�� 
$str
�� E
,
��E F
column
�� 
:
�� 
x
��  !
=>
��" $
x
��% &
.
��& '
UserId
��' -
,
��- .
principalTable
�� &
:
��& '
$str
��( 5
,
��5 6
principalColumn
�� '
:
��' (
$str
��) -
,
��- .
onDelete
��  
:
��  !
ReferentialAction
��" 3
.
��3 4
Cascade
��4 ;
)
��; <
;
��< =
}
�� 
)
�� 
;
�� 
migrationBuilder
�� 
.
�� 
CreateTable
�� (
(
��( )
name
�� 
:
�� 
$str
�� (
,
��( )
columns
�� 
:
�� 
table
�� 
=>
�� !
new
��" %
{
�� 
UserId
�� 
=
�� 
table
�� "
.
��" #
Column
��# )
<
��) *
string
��* 0
>
��0 1
(
��1 2
type
��2 6
:
��6 7
$str
��8 G
,
��G H
nullable
��I Q
:
��Q R
false
��S X
)
��X Y
,
��Y Z
LoginProvider
�� !
=
��" #
table
��$ )
.
��) *
Column
��* 0
<
��0 1
string
��1 7
>
��7 8
(
��8 9
type
��9 =
:
��= >
$str
��? N
,
��N O
nullable
��P X
:
��X Y
false
��Z _
)
��_ `
,
��` a
Name
�� 
=
�� 
table
��  
.
��  !
Column
��! '
<
��' (
string
��( .
>
��. /
(
��/ 0
type
��0 4
:
��4 5
$str
��6 E
,
��E F
nullable
��G O
:
��O P
false
��Q V
)
��V W
,
��W X
Value
�� 
=
�� 
table
�� !
.
��! "
Column
��" (
<
��( )
string
��) /
>
��/ 0
(
��0 1
type
��1 5
:
��5 6
$str
��7 F
,
��F G
nullable
��H P
:
��P Q
true
��R V
)
��V W
}
�� 
,
�� 
constraints
�� 
:
�� 
table
�� "
=>
��# %
{
�� 
table
�� 
.
�� 

PrimaryKey
�� $
(
��$ %
$str
��% :
,
��: ;
x
��< =
=>
��> @
new
��A D
{
��E F
x
��G H
.
��H I
UserId
��I O
,
��O P
x
��Q R
.
��R S
LoginProvider
��S `
,
��` a
x
��b c
.
��c d
Name
��d h
}
��i j
)
��j k
;
��k l
table
�� 
.
�� 

ForeignKey
�� $
(
��$ %
name
�� 
:
�� 
$str
�� F
,
��F G
column
�� 
:
�� 
x
��  !
=>
��" $
x
��% &
.
��& '
UserId
��' -
,
��- .
principalTable
�� &
:
��& '
$str
��( 5
,
��5 6
principalColumn
�� '
:
��' (
$str
��) -
,
��- .
onDelete
��  
:
��  !
ReferentialAction
��" 3
.
��3 4
Cascade
��4 ;
)
��; <
;
��< =
}
�� 
)
�� 
;
�� 
migrationBuilder
�� 
.
�� 
CreateTable
�� (
(
��( )
name
�� 
:
�� 
$str
�� )
,
��) *
columns
�� 
:
�� 
table
�� 
=>
�� !
new
��" %
{
�� 
	ContestId
�� 
=
�� 
table
��  %
.
��% &
Column
��& ,
<
��, -
string
��- 3
>
��3 4
(
��4 5
type
��5 9
:
��9 :
$str
��; J
,
��J K
nullable
��L T
:
��T U
false
��V [
)
��[ \
,
��\ ]
Id
�� 
=
�� 
table
�� 
.
�� 
Column
�� %
<
��% &
string
��& ,
>
��, -
(
��- .
type
��. 2
:
��2 3
$str
��4 C
,
��C D
nullable
��E M
:
��M N
false
��O T
)
��T U
,
��U V
	CreatedAt
�� 
=
�� 
table
��  %
.
��% &
Column
��& ,
<
��, -
DateTime
��- 5
>
��5 6
(
��6 7
type
��7 ;
:
��; <
$str
��= H
,
��H I
nullable
��J R
:
��R S
false
��T Y
)
��Y Z
,
��Z [
	AppUserId
�� 
=
�� 
table
��  %
.
��% &
Column
��& ,
<
��, -
string
��- 3
>
��3 4
(
��4 5
type
��5 9
:
��9 :
$str
��; J
,
��J K
nullable
��L T
:
��T U
false
��V [
)
��[ \
}
�� 
,
�� 
constraints
�� 
:
�� 
table
�� "
=>
��# %
{
�� 
table
�� 
.
�� 

PrimaryKey
�� $
(
��$ %
$str
��% ;
,
��; <
x
��= >
=>
��? A
new
��B E
{
��F G
x
��H I
.
��I J
Id
��J L
,
��L M
x
��N O
.
��O P
	ContestId
��P Y
}
��Z [
)
��[ \
;
��\ ]
table
�� 
.
�� 

ForeignKey
�� $
(
��$ %
name
�� 
:
�� 
$str
�� J
,
��J K
column
�� 
:
�� 
x
��  !
=>
��" $
x
��% &
.
��& '
	AppUserId
��' 0
,
��0 1
principalTable
�� &
:
��& '
$str
��( 5
,
��5 6
principalColumn
�� '
:
��' (
$str
��) -
,
��- .
onDelete
��  
:
��  !
ReferentialAction
��" 3
.
��3 4
Cascade
��4 ;
)
��; <
;
��< =
table
�� 
.
�� 

ForeignKey
�� $
(
��$ %
name
�� 
:
�� 
$str
�� G
,
��G H
column
�� 
:
�� 
x
��  !
=>
��" $
x
��% &
.
��& '
	ContestId
��' 0
,
��0 1
principalTable
�� &
:
��& '
$str
��( 2
,
��2 3
principalColumn
�� '
:
��' (
$str
��) 4
,
��4 5
onDelete
��  
:
��  !
ReferentialAction
��" 3
.
��3 4
Cascade
��4 ;
)
��; <
;
��< =
}
�� 
)
�� 
;
�� 
migrationBuilder
�� 
.
�� 
CreateTable
�� (
(
��( )
name
�� 
:
�� 
$str
�� '
,
��' (
columns
�� 
:
�� 
table
�� 
=>
�� !
new
��" %
{
�� 
	ContestId
�� 
=
�� 
table
��  %
.
��% &
Column
��& ,
<
��, -
string
��- 3
>
��3 4
(
��4 5
type
��5 9
:
��9 :
$str
��; J
,
��J K
nullable
��L T
:
��T U
false
��V [
)
��[ \
,
��\ ]
	ProblemId
�� 
=
�� 
table
��  %
.
��% &
Column
��& ,
<
��, -
string
��- 3
>
��3 4
(
��4 5
type
��5 9
:
��9 :
$str
��; J
,
��J K
nullable
��L T
:
��T U
false
��V [
)
��[ \
}
�� 
,
�� 
constraints
�� 
:
�� 
table
�� "
=>
��# %
{
�� 
table
�� 
.
�� 

PrimaryKey
�� $
(
��$ %
$str
��% 9
,
��9 :
x
��; <
=>
��= ?
new
��@ C
{
��D E
x
��F G
.
��G H
	ContestId
��H Q
,
��Q R
x
��S T
.
��T U
	ProblemId
��U ^
}
��_ `
)
��` a
;
��a b
table
�� 
.
�� 

ForeignKey
�� $
(
��$ %
name
�� 
:
�� 
$str
�� E
,
��E F
column
�� 
:
�� 
x
��  !
=>
��" $
x
��% &
.
��& '
	ContestId
��' 0
,
��0 1
principalTable
�� &
:
��& '
$str
��( 2
,
��2 3
principalColumn
�� '
:
��' (
$str
��) 4
,
��4 5
onDelete
��  
:
��  !
ReferentialAction
��" 3
.
��3 4
Cascade
��4 ;
)
��; <
;
��< =
table
�� 
.
�� 

ForeignKey
�� $
(
��$ %
name
�� 
:
�� 
$str
�� E
,
��E F
column
�� 
:
�� 
x
��  !
=>
��" $
x
��% &
.
��& '
	ProblemId
��' 0
,
��0 1
principalTable
�� &
:
��& '
$str
��( 2
,
��2 3
principalColumn
�� '
:
��' (
$str
��) 4
,
��4 5
onDelete
��  
:
��  !
ReferentialAction
��" 3
.
��3 4
Cascade
��4 ;
)
��; <
;
��< =
}
�� 
)
�� 
;
�� 
migrationBuilder
�� 
.
�� 
CreateTable
�� (
(
��( )
name
�� 
:
�� 
$str
�� #
,
��# $
columns
�� 
:
�� 
table
�� 
=>
�� !
new
��" %
{
�� 
SubmissionId
��  
=
��! "
table
��# (
.
��( )
Column
��) /
<
��/ 0
string
��0 6
>
��6 7
(
��7 8
type
��8 <
:
��< =
$str
��> M
,
��M N
nullable
��O W
:
��W X
false
��Y ^
)
��^ _
,
��_ `
Point
�� 
=
�� 
table
�� !
.
��! "
Column
��" (
<
��( )
int
��) ,
>
��, -
(
��- .
type
��. 2
:
��2 3
$str
��4 9
,
��9 :
nullable
��; C
:
��C D
false
��E J
)
��J K
,
��K L

SourceCode
�� 
=
��  
table
��! &
.
��& '
Column
��' -
<
��- .
string
��. 4
>
��4 5
(
��5 6
type
��6 :
:
��: ;
$str
��< K
,
��K L
nullable
��M U
:
��U V
false
��W \
)
��\ ]
,
��] ^
Status
�� 
=
�� 
table
�� "
.
��" #
Column
��# )
<
��) *
string
��* 0
>
��0 1
(
��1 2
type
��2 6
:
��6 7
$str
��8 F
,
��F G
	maxLength
��H Q
:
��Q R
$num
��S U
,
��U V
nullable
��W _
:
��_ `
false
��a f
)
��f g
,
��g h
ExecuteTime
�� 
=
��  !
table
��" '
.
��' (
Column
��( .
<
��. /
double
��/ 5
>
��5 6
(
��6 7
type
��7 ;
:
��; <
$str
��= D
,
��D E
nullable
��F N
:
��N O
false
��P U
)
��U V
,
��V W

MemoryUsed
�� 
=
��  
table
��! &
.
��& '
Column
��' -
<
��- .
int
��. 1
>
��1 2
(
��2 3
type
��3 7
:
��7 8
$str
��9 >
,
��> ?
nullable
��@ H
:
��H I
false
��J O
)
��O P
,
��P Q
Language
�� 
=
�� 
table
�� $
.
��$ %
Column
��% +
<
��+ ,
string
��, 2
>
��2 3
(
��3 4
type
��4 8
:
��8 9
$str
��: H
,
��H I
	maxLength
��J S
:
��S T
$num
��U W
,
��W X
nullable
��Y a
:
��a b
false
��c h
)
��h i
,
��i j
SubmittedAt
�� 
=
��  !
table
��" '
.
��' (
Column
��( .
<
��. /
DateTime
��/ 7
>
��7 8
(
��8 9
type
��9 =
:
��= >
$str
��? J
,
��J K
nullable
��L T
:
��T U
false
��V [
)
��[ \
,
��\ ]
	ProblemId
�� 
=
�� 
table
��  %
.
��% &
Column
��& ,
<
��, -
string
��- 3
>
��3 4
(
��4 5
type
��5 9
:
��9 :
$str
��; J
,
��J K
nullable
��L T
:
��T U
false
��V [
)
��[ \
,
��\ ]
	AppUserId
�� 
=
�� 
table
��  %
.
��% &
Column
��& ,
<
��, -
string
��- 3
>
��3 4
(
��4 5
type
��5 9
:
��9 :
$str
��; J
,
��J K
nullable
��L T
:
��T U
false
��V [
)
��[ \
}
�� 
,
�� 
constraints
�� 
:
�� 
table
�� "
=>
��# %
{
�� 
table
�� 
.
�� 

PrimaryKey
�� $
(
��$ %
$str
��% 5
,
��5 6
x
��7 8
=>
��9 ;
x
��< =
.
��= >
SubmissionId
��> J
)
��J K
;
��K L
table
�� 
.
�� 

ForeignKey
�� $
(
��$ %
name
�� 
:
�� 
$str
�� D
,
��D E
column
�� 
:
�� 
x
��  !
=>
��" $
x
��% &
.
��& '
	AppUserId
��' 0
,
��0 1
principalTable
�� &
:
��& '
$str
��( 5
,
��5 6
principalColumn
�� '
:
��' (
$str
��) -
,
��- .
onDelete
��  
:
��  !
ReferentialAction
��" 3
.
��3 4
Cascade
��4 ;
)
��; <
;
��< =
table
�� 
.
�� 

ForeignKey
�� $
(
��$ %
name
�� 
:
�� 
$str
�� A
,
��A B
column
�� 
:
�� 
x
��  !
=>
��" $
x
��% &
.
��& '
	ProblemId
��' 0
,
��0 1
principalTable
�� &
:
��& '
$str
��( 2
,
��2 3
principalColumn
�� '
:
��' (
$str
��) 4
,
��4 5
onDelete
��  
:
��  !
ReferentialAction
��" 3
.
��3 4
Cascade
��4 ;
)
��; <
;
��< =
}
�� 
)
�� 
;
�� 
migrationBuilder
�� 
.
�� 
CreateTable
�� (
(
��( )
name
�� 
:
�� 
$str
�� !
,
��! "
columns
�� 
:
�� 
table
�� 
=>
�� !
new
��" %
{
�� 

TestCaseId
�� 
=
��  
table
��! &
.
��& '
Column
��' -
<
��- .
string
��. 4
>
��4 5
(
��5 6
type
��6 :
:
��: ;
$str
��< K
,
��K L
nullable
��M U
:
��U V
false
��W \
)
��\ ]
,
��] ^
	ProblemId
�� 
=
�� 
table
��  %
.
��% &
Column
��& ,
<
��, -
string
��- 3
>
��3 4
(
��4 5
type
��5 9
:
��9 :
$str
��; J
,
��J K
nullable
��L T
:
��T U
false
��V [
)
��[ \
,
��\ ]
TestCaseName
��  
=
��! "
table
��# (
.
��( )
Column
��) /
<
��/ 0
string
��0 6
>
��6 7
(
��7 8
type
��8 <
:
��< =
$str
��> M
,
��M N
	maxLength
��O X
:
��X Y
$num
��Z ]
,
��] ^
nullable
��_ g
:
��g h
false
��i n
)
��n o
,
��o p
Input
�� 
=
�� 
table
�� !
.
��! "
Column
��" (
<
��( )
string
��) /
>
��/ 0
(
��0 1
type
��1 5
:
��5 6
$str
��7 F
,
��F G
nullable
��H P
:
��P Q
false
��R W
)
��W X
,
��X Y
Output
�� 
=
�� 
table
�� "
.
��" #
Column
��# )
<
��) *
string
��* 0
>
��0 1
(
��1 2
type
��2 6
:
��6 7
$str
��8 G
,
��G H
nullable
��I Q
:
��Q R
false
��S X
)
��X Y
}
�� 
,
�� 
constraints
�� 
:
�� 
table
�� "
=>
��# %
{
�� 
table
�� 
.
�� 

PrimaryKey
�� $
(
��$ %
$str
��% 3
,
��3 4
x
��5 6
=>
��7 9
x
��: ;
.
��; <

TestCaseId
��< F
)
��F G
;
��G H
table
�� 
.
�� 

ForeignKey
�� $
(
��$ %
name
�� 
:
�� 
$str
�� ?
,
��? @
column
�� 
:
�� 
x
��  !
=>
��" $
x
��% &
.
��& '
	ProblemId
��' 0
,
��0 1
principalTable
�� &
:
��& '
$str
��( 2
,
��2 3
principalColumn
�� '
:
��' (
$str
��) 4
,
��4 5
onDelete
��  
:
��  !
ReferentialAction
��" 3
.
��3 4
Cascade
��4 ;
)
��; <
;
��< =
}
�� 
)
�� 
;
�� 
migrationBuilder
�� 
.
�� 
CreateTable
�� (
(
��( )
name
�� 
:
�� 
$str
�� (
,
��( )
columns
�� 
:
�� 
table
�� 
=>
�� !
new
��" %
{
�� 

TestCaseId
�� 
=
��  
table
��! &
.
��& '
Column
��' -
<
��- .
string
��. 4
>
��4 5
(
��5 6
type
��6 :
:
��: ;
$str
��< K
,
��K L
nullable
��M U
:
��U V
false
��W \
)
��\ ]
,
��] ^
ExecutionTime
�� !
=
��" #
table
��$ )
.
��) *
Column
��* 0
<
��0 1
double
��1 7
>
��7 8
(
��8 9
type
��9 =
:
��= >
$str
��? F
,
��F G
nullable
��H P
:
��P Q
false
��R W
)
��W X
,
��X Y
MemoryUsage
�� 
=
��  !
table
��" '
.
��' (
Column
��( .
<
��. /
int
��/ 2
>
��2 3
(
��3 4
type
��4 8
:
��8 9
$str
��: ?
,
��? @
nullable
��A I
:
��I J
false
��K P
)
��P Q
,
��Q R
Result
�� 
=
�� 
table
�� "
.
��" #
Column
��# )
<
��) *
string
��* 0
>
��0 1
(
��1 2
type
��2 6
:
��6 7
$str
��8 F
,
��F G
	maxLength
��H Q
:
��Q R
$num
��S U
,
��U V
nullable
��W _
:
��_ `
false
��a f
)
��f g
,
��g h
Log
�� 
=
�� 
table
�� 
.
��  
Column
��  &
<
��& '
string
��' -
>
��- .
(
��. /
type
��/ 3
:
��3 4
$str
��5 D
,
��D E
nullable
��F N
:
��N O
false
��P U
)
��U V
,
��V W
SubmissionId
��  
=
��! "
table
��# (
.
��( )
Column
��) /
<
��/ 0
string
��0 6
>
��6 7
(
��7 8
type
��8 <
:
��< =
$str
��> M
,
��M N
nullable
��O W
:
��W X
false
��Y ^
)
��^ _
}
�� 
,
�� 
constraints
�� 
:
�� 
table
�� "
=>
��# %
{
�� 
table
�� 
.
�� 

PrimaryKey
�� $
(
��$ %
$str
��% :
,
��: ;
x
��< =
=>
��> @
x
��A B
.
��B C

TestCaseId
��C M
)
��M N
;
��N O
table
�� 
.
�� 

ForeignKey
�� $
(
��$ %
name
�� 
:
�� 
$str
�� L
,
��L M
column
�� 
:
�� 
x
��  !
=>
��" $
x
��% &
.
��& '
SubmissionId
��' 3
,
��3 4
principalTable
�� &
:
��& '
$str
��( 5
,
��5 6
principalColumn
�� '
:
��' (
$str
��) 7
,
��7 8
onDelete
��  
:
��  !
ReferentialAction
��" 3
.
��3 4
Cascade
��4 ;
)
��; <
;
��< =
}
�� 
)
�� 
;
�� 
migrationBuilder
�� 
.
�� 

InsertData
�� '
(
��' (
table
�� 
:
�� 
$str
�� $
,
��$ %
columns
�� 
:
�� 
new
�� 
[
�� 
]
�� 
{
��  
$str
��! %
,
��% &
$str
��' 9
,
��9 :
$str
��; A
,
��A B
$str
��C S
}
��T U
,
��U V
values
�� 
:
�� 
new
�� 
object
�� "
[
��" #
,
��# $
]
��$ %
{
�� 
{
�� 
$str
�� <
,
��< =
null
��> B
,
��B C
$str
��D K
,
��K L
$str
��M T
}
��U V
,
��V W
{
�� 
$str
�� <
,
��< =
null
��> B
,
��B C
$str
��D K
,
��K L
$str
��M T
}
��U V
,
��V W
{
�� 
$str
�� <
,
��< =
null
��> B
,
��B C
$str
��D J
,
��J K
$str
��L R
}
��S T
}
�� 
)
�� 
;
�� 
migrationBuilder
�� 
.
�� 
CreateIndex
�� (
(
��( )
name
�� 
:
�� 
$str
�� 2
,
��2 3
table
�� 
:
�� 
$str
�� )
,
��) *
column
�� 
:
�� 
$str
��  
)
��  !
;
��! "
migrationBuilder
�� 
.
�� 
CreateIndex
�� (
(
��( )
name
�� 
:
�� 
$str
�� %
,
��% &
table
�� 
:
�� 
$str
�� $
,
��$ %
column
�� 
:
�� 
$str
�� (
,
��( )
unique
�� 
:
�� 
true
�� 
,
�� 
filter
�� 
:
�� 
$str
�� 6
)
��6 7
;
��7 8
migrationBuilder
�� 
.
�� 
CreateIndex
�� (
(
��( )
name
�� 
:
�� 
$str
�� 2
,
��2 3
table
�� 
:
�� 
$str
�� )
,
��) *
column
�� 
:
�� 
$str
��  
)
��  !
;
��! "
migrationBuilder
�� 
.
�� 
CreateIndex
�� (
(
��( )
name
�� 
:
�� 
$str
�� 2
,
��2 3
table
�� 
:
�� 
$str
�� )
,
��) *
column
�� 
:
�� 
$str
��  
)
��  !
;
��! "
migrationBuilder
�� 
.
�� 
CreateIndex
�� (
(
��( )
name
�� 
:
�� 
$str
�� 1
,
��1 2
table
�� 
:
�� 
$str
�� (
,
��( )
column
�� 
:
�� 
$str
��  
)
��  !
;
��! "
migrationBuilder
�� 
.
�� 
CreateIndex
�� (
(
��( )
name
�� 
:
�� 
$str
�� "
,
��" #
table
�� 
:
�� 
$str
�� $
,
��$ %
column
�� 
:
�� 
$str
�� )
)
��) *
;
��* +
migrationBuilder
�� 
.
�� 
CreateIndex
�� (
(
��( )
name
�� 
:
�� 
$str
�� %
,
��% &
table
�� 
:
�� 
$str
�� $
,
��$ %
column
�� 
:
�� 
$str
�� ,
,
��, -
unique
�� 
:
�� 
true
�� 
,
�� 
filter
�� 
:
�� 
$str
�� :
)
��: ;
;
��; <
migrationBuilder
�� 
.
�� 
CreateIndex
�� (
(
��( )
name
�� 
:
�� 
$str
�� 4
,
��4 5
table
�� 
:
�� 
$str
�� (
,
��( )
column
�� 
:
�� 
$str
�� #
)
��# $
;
��$ %
migrationBuilder
�� 
.
�� 
CreateIndex
�� (
(
��( )
name
�� 
:
�� 
$str
�� 6
,
��6 7
table
�� 
:
�� 
$str
�� *
,
��* +
column
�� 
:
�� 
$str
�� #
)
��# $
;
��$ %
migrationBuilder
�� 
.
�� 
CreateIndex
�� (
(
��( )
name
�� 
:
�� 
$str
�� 6
,
��6 7
table
�� 
:
�� 
$str
�� *
,
��* +
column
�� 
:
�� 
$str
�� #
)
��# $
;
��$ %
migrationBuilder
�� 
.
�� 
CreateIndex
�� (
(
��( )
name
�� 
:
�� 
$str
�� 0
,
��0 1
table
�� 
:
�� 
$str
�� $
,
��$ %
column
�� 
:
�� 
$str
�� #
)
��# $
;
��$ %
migrationBuilder
�� 
.
�� 
CreateIndex
�� (
(
��( )
name
�� 
:
�� 
$str
�� 0
,
��0 1
table
�� 
:
�� 
$str
�� $
,
��$ %
column
�� 
:
�� 
$str
�� #
)
��# $
;
��$ %
migrationBuilder
�� 
.
�� 
CreateIndex
�� (
(
��( )
name
�� 
:
�� 
$str
�� .
,
��. /
table
�� 
:
�� 
$str
�� "
,
��" #
column
�� 
:
�� 
$str
�� #
)
��# $
;
��$ %
migrationBuilder
�� 
.
�� 
CreateIndex
�� (
(
��( )
name
�� 
:
�� 
$str
�� 8
,
��8 9
table
�� 
:
�� 
$str
�� )
,
��) *
column
�� 
:
�� 
$str
�� &
)
��& '
;
��' (
}
�� 	
	protected
�� 
override
�� 
void
�� 
Down
��  $
(
��$ %
MigrationBuilder
��% 5
migrationBuilder
��6 F
)
��F G
{
�� 	
migrationBuilder
�� 
.
�� 
	DropTable
�� &
(
��& '
name
�� 
:
�� 
$str
�� (
)
��( )
;
��) *
migrationBuilder
�� 
.
�� 
	DropTable
�� &
(
��& '
name
�� 
:
�� 
$str
�� (
)
��( )
;
��) *
migrationBuilder
�� 
.
�� 
	DropTable
�� &
(
��& '
name
�� 
:
�� 
$str
�� (
)
��( )
;
��) *
migrationBuilder
�� 
.
�� 
	DropTable
�� &
(
��& '
name
�� 
:
�� 
$str
�� '
)
��' (
;
��( )
migrationBuilder
�� 
.
�� 
	DropTable
�� &
(
��& '
name
�� 
:
�� 
$str
�� (
)
��( )
;
��) *
migrationBuilder
�� 
.
�� 
	DropTable
�� &
(
��& '
name
�� 
:
�� 
$str
�� '
)
��' (
;
��( )
migrationBuilder
�� 
.
�� 
	DropTable
�� &
(
��& '
name
�� 
:
�� 
$str
�� )
)
��) *
;
��* +
migrationBuilder
�� 
.
�� 
	DropTable
�� &
(
��& '
name
�� 
:
�� 
$str
�� !
)
��! "
;
��" #
migrationBuilder
�� 
.
�� 
	DropTable
�� &
(
��& '
name
�� 
:
�� 
$str
�� (
)
��( )
;
��) *
migrationBuilder
�� 
.
�� 
	DropTable
�� &
(
��& '
name
�� 
:
�� 
$str
�� #
)
��# $
;
��$ %
migrationBuilder
�� 
.
�� 
	DropTable
�� &
(
��& '
name
�� 
:
�� 
$str
��  
)
��  !
;
��! "
migrationBuilder
�� 
.
�� 
	DropTable
�� &
(
��& '
name
�� 
:
�� 
$str
�� #
)
��# $
;
��$ %
migrationBuilder
�� 
.
�� 
	DropTable
�� &
(
��& '
name
�� 
:
�� 
$str
�� #
)
��# $
;
��$ %
migrationBuilder
�� 
.
�� 
	DropTable
�� &
(
��& '
name
�� 
:
�� 
$str
��  
)
��  !
;
��! "
}
�� 	
}
�� 
}�� ��
bD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Migrations\20250205025534_BlogTable.cs
	namespace 	
api
 
. 

Migrations 
{ 
public		 

partial		 
class		 
	BlogTable		 "
:		# $
	Migration		% .
{

 
	protected 
override 
void 
Up  "
(" #
MigrationBuilder# 3
migrationBuilder4 D
)D E
{ 	
migrationBuilder 
. 
CreateTable (
(( )
name 
: 
$str 
, 
columns 
: 
table 
=> !
new" %
{ 
UserId 
= 
table "
." #
Column# )
<) *
int* -
>- .
(. /
type/ 3
:3 4
$str5 :
,: ;
nullable< D
:D E
falseF K
)K L
. 

Annotation #
(# $
$str$ 8
,8 9
$str: @
)@ A
,A B
Username 
= 
table $
.$ %
Column% +
<+ ,
string, 2
>2 3
(3 4
type4 8
:8 9
$str: I
,I J
nullableK S
:S T
falseU Z
)Z [
,[ \
Password 
= 
table $
.$ %
Column% +
<+ ,
string, 2
>2 3
(3 4
type4 8
:8 9
$str: I
,I J
nullableK S
:S T
falseU Z
)Z [
,[ \
Email 
= 
table !
.! "
Column" (
<( )
string) /
>/ 0
(0 1
type1 5
:5 6
$str7 F
,F G
nullableH P
:P Q
falseR W
)W X
,X Y
FullName 
= 
table $
.$ %
Column% +
<+ ,
string, 2
>2 3
(3 4
type4 8
:8 9
$str: I
,I J
nullableK S
:S T
falseU Z
)Z [
,[ \
Address 
= 
table #
.# $
Column$ *
<* +
string+ 1
>1 2
(2 3
type3 7
:7 8
$str9 H
,H I
nullableJ R
:R S
falseT Y
)Y Z
,Z [
Avatar 
= 
table "
." #
Column# )
<) *
string* 0
>0 1
(1 2
type2 6
:6 7
$str8 G
,G H
nullableI Q
:Q R
falseS X
)X Y
,Y Z
DateOfBirth 
=  !
table" '
.' (
Column( .
<. /
DateTime/ 7
>7 8
(8 9
type9 =
:= >
$str? J
,J K
nullableL T
:T U
falseV [
)[ \
,\ ]
Phone 
= 
table !
.! "
Column" (
<( )
string) /
>/ 0
(0 1
type1 5
:5 6
$str7 F
,F G
nullableH P
:P Q
falseR W
)W X
,X Y
	CreatedAt 
= 
table  %
.% &
Column& ,
<, -
DateTime- 5
>5 6
(6 7
type7 ;
:; <
$str= H
,H I
nullableJ R
:R S
falseT Y
)Y Z
,Z [
LastLoginAt 
=  !
table" '
.' (
Column( .
<. /
DateTime/ 7
>7 8
(8 9
type9 =
:= >
$str? J
,J K
nullableL T
:T U
trueV Z
)Z [
,[ \
TotalSolved 
=  !
table" '
.' (
Column( .
<. /
int/ 2
>2 3
(3 4
type4 8
:8 9
$str: ?
,? @
nullableA I
:I J
falseK P
)P Q
,Q R
TotalSubmissions $
=% &
table' ,
., -
Column- 3
<3 4
int4 7
>7 8
(8 9
type9 =
:= >
$str? D
,D E
nullableF N
:N O
falseP U
)U V
,V W
LastSolvedAt    
=  ! "
table  # (
.  ( )
Column  ) /
<  / 0
DateTime  0 8
>  8 9
(  9 :
type  : >
:  > ?
$str  @ K
,  K L
nullable  M U
:  U V
true  W [
)  [ \
}!! 
,!! 
constraints"" 
:"" 
table"" "
=>""# %
{## 
table$$ 
.$$ 

PrimaryKey$$ $
($$$ %
$str$$% .
,$$. /
x$$0 1
=>$$2 4
x$$5 6
.$$6 7
UserId$$7 =
)$$= >
;$$> ?
}%% 
)%% 
;%% 
migrationBuilder'' 
.'' 
CreateTable'' (
(''( )
name(( 
:(( 
$str(( 
,(( 
columns)) 
:)) 
table)) 
=>)) !
new))" %
{** 
ID++ 
=++ 
table++ 
.++ 
Column++ %
<++% &
int++& )
>++) *
(++* +
type+++ /
:++/ 0
$str++1 6
,++6 7
nullable++8 @
:++@ A
false++B G
)++G H
.,, 

Annotation,, #
(,,# $
$str,,$ 8
,,,8 9
$str,,: @
),,@ A
,,,A B
UserId-- 
=-- 
table-- "
.--" #
Column--# )
<--) *
int--* -
>--- .
(--. /
type--/ 3
:--3 4
$str--5 :
,--: ;
nullable--< D
:--D E
true--F J
)--J K
,--K L
	GuestName.. 
=.. 
table..  %
...% &
Column..& ,
<.., -
string..- 3
>..3 4
(..4 5
type..5 9
:..9 :
$str..; J
,..J K
nullable..L T
:..T U
true..V Z
)..Z [
,..[ \

GuestEmail// 
=//  
table//! &
.//& '
Column//' -
<//- .
string//. 4
>//4 5
(//5 6
type//6 :
://: ;
$str//< K
,//K L
nullable//M U
://U V
true//W [
)//[ \
,//\ ]
	Thumbnail00 
=00 
table00  %
.00% &
Column00& ,
<00, -
string00- 3
>003 4
(004 5
type005 9
:009 :
$str00; J
,00J K
nullable00L T
:00T U
false00V [
)00[ \
,00\ ]
title11 
=11 
table11 !
.11! "
Column11" (
<11( )
string11) /
>11/ 0
(110 1
type111 5
:115 6
$str117 F
,11F G
nullable11H P
:11P Q
false11R W
)11W X
,11X Y
description22 
=22  !
table22" '
.22' (
Column22( .
<22. /
string22/ 5
>225 6
(226 7
type227 ;
:22; <
$str22= L
,22L M
nullable22N V
:22V W
false22X ]
)22] ^
,22^ _
Content33 
=33 
table33 #
.33# $
Column33$ *
<33* +
string33+ 1
>331 2
(332 3
type333 7
:337 8
$str339 H
,33H I
nullable33J R
:33R S
false33T Y
)33Y Z
,33Z [
Status44 
=44 
table44 "
.44" #
Column44# )
<44) *
string44* 0
>440 1
(441 2
type442 6
:446 7
$str448 G
,44G H
nullable44I Q
:44Q R
false44S X
)44X Y
,44Y Z
CreateOn55 
=55 
table55 $
.55$ %
Column55% +
<55+ ,
DateTime55, 4
>554 5
(555 6
type556 :
:55: ;
$str55< G
,55G H
nullable55I Q
:55Q R
false55S X
)55X Y
,55Y Z

DatePublic66 
=66  
table66! &
.66& '
Column66' -
<66- .
DateTime66. 6
>666 7
(667 8
type668 <
:66< =
$str66> I
,66I J
nullable66K S
:66S T
true66U Y
)66Y Z
}77 
,77 
constraints88 
:88 
table88 "
=>88# %
{99 
table:: 
.:: 

PrimaryKey:: $
(::$ %
$str::% /
,::/ 0
x::1 2
=>::3 5
x::6 7
.::7 8
ID::8 :
)::: ;
;::; <
table;; 
.;; 

ForeignKey;; $
(;;$ %
name<< 
:<< 
$str<< 4
,<<4 5
column== 
:== 
x==  !
=>==" $
x==% &
.==& '
UserId==' -
,==- .
principalTable>> &
:>>& '
$str>>( .
,>>. /
principalColumn?? '
:??' (
$str??) 1
)??1 2
;??2 3
}@@ 
)@@ 
;@@ 
migrationBuilderBB 
.BB 
CreateTableBB (
(BB( )
nameCC 
:CC 
$strCC $
,CC$ %
columnsDD 
:DD 
tableDD 
=>DD !
newDD" %
{EE 
IDFF 
=FF 
tableFF 
.FF 
ColumnFF %
<FF% &
intFF& )
>FF) *
(FF* +
typeFF+ /
:FF/ 0
$strFF1 6
,FF6 7
nullableFF8 @
:FF@ A
falseFFB G
)FFG H
.GG 

AnnotationGG #
(GG# $
$strGG$ 8
,GG8 9
$strGG: @
)GG@ A
,GGA B
CategoryNameHH  
=HH! "
tableHH# (
.HH( )
ColumnHH) /
<HH/ 0
stringHH0 6
>HH6 7
(HH7 8
typeHH8 <
:HH< =
$strHH> M
,HHM N
nullableHHO W
:HHW X
falseHHY ^
)HH^ _
,HH_ `
BlogIdII 
=II 
tableII "
.II" #
ColumnII# )
<II) *
intII* -
>II- .
(II. /
typeII/ 3
:II3 4
$strII5 :
,II: ;
nullableII< D
:IID E
trueIIF J
)IIJ K
}JJ 
,JJ 
constraintsKK 
:KK 
tableKK "
=>KK# %
{LL 
tableMM 
.MM 

PrimaryKeyMM $
(MM$ %
$strMM% 6
,MM6 7
xMM8 9
=>MM: <
xMM= >
.MM> ?
IDMM? A
)MMA B
;MMB C
tableNN 
.NN 

ForeignKeyNN $
(NN$ %
nameOO 
:OO 
$strOO <
,OO< =
columnPP 
:PP 
xPP  !
=>PP" $
xPP% &
.PP& '
BlogIdPP' -
,PP- .
principalTableQQ &
:QQ& '
$strQQ( /
,QQ/ 0
principalColumnRR '
:RR' (
$strRR) -
)RR- .
;RR. /
}SS 
)SS 
;SS 
migrationBuilderUU 
.UU 
CreateTableUU (
(UU( )
nameVV 
:VV 
$strVV #
,VV# $
columnsWW 
:WW 
tableWW 
=>WW !
newWW" %
{XX 
IDYY 
=YY 
tableYY 
.YY 
ColumnYY %
<YY% &
intYY& )
>YY) *
(YY* +
typeYY+ /
:YY/ 0
$strYY1 6
,YY6 7
nullableYY8 @
:YY@ A
falseYYB G
)YYG H
.ZZ 

AnnotationZZ #
(ZZ# $
$strZZ$ 8
,ZZ8 9
$strZZ: @
)ZZ@ A
,ZZA B
title[[ 
=[[ 
table[[ !
.[[! "
Column[[" (
<[[( )
string[[) /
>[[/ 0
([[0 1
type[[1 5
:[[5 6
$str[[7 F
,[[F G
nullable[[H P
:[[P Q
false[[R W
)[[W X
,[[X Y
content\\ 
=\\ 
table\\ #
.\\# $
Column\\$ *
<\\* +
string\\+ 1
>\\1 2
(\\2 3
type\\3 7
:\\7 8
$str\\9 H
,\\H I
nullable\\J R
:\\R S
false\\T Y
)\\Y Z
,\\Z [
CreateOn]] 
=]] 
table]] $
.]]$ %
Column]]% +
<]]+ ,
DateTime]], 4
>]]4 5
(]]5 6
type]]6 :
:]]: ;
$str]]< G
,]]G H
nullable]]I Q
:]]Q R
false]]S X
)]]X Y
,]]Y Z
BlogId^^ 
=^^ 
table^^ "
.^^" #
Column^^# )
<^^) *
int^^* -
>^^- .
(^^. /
type^^/ 3
:^^3 4
$str^^5 :
,^^: ;
nullable^^< D
:^^D E
true^^F J
)^^J K
}__ 
,__ 
constraints`` 
:`` 
table`` "
=>``# %
{aa 
tablebb 
.bb 

PrimaryKeybb $
(bb$ %
$strbb% 5
,bb5 6
xbb7 8
=>bb9 ;
xbb< =
.bb= >
IDbb> @
)bb@ A
;bbA B
tablecc 
.cc 

ForeignKeycc $
(cc$ %
namedd 
:dd 
$strdd ;
,dd; <
columnee 
:ee 
xee  !
=>ee" $
xee% &
.ee& '
BlogIdee' -
,ee- .
principalTableff &
:ff& '
$strff( /
,ff/ 0
principalColumngg '
:gg' (
$strgg) -
)gg- .
;gg. /
}hh 
)hh 
;hh 
migrationBuilderjj 
.jj 
CreateTablejj (
(jj( )
namekk 
:kk 
$strkk !
,kk! "
columnsll 
:ll 
tablell 
=>ll !
newll" %
{mm 
IDnn 
=nn 
tablenn 
.nn 
Columnnn %
<nn% &
intnn& )
>nn) *
(nn* +
typenn+ /
:nn/ 0
$strnn1 6
,nn6 7
nullablenn8 @
:nn@ A
falsennB G
)nnG H
.oo 

Annotationoo #
(oo# $
$stroo$ 8
,oo8 9
$stroo: @
)oo@ A
,ooA B
Urlpp 
=pp 
tablepp 
.pp  
Columnpp  &
<pp& '
stringpp' -
>pp- .
(pp. /
typepp/ 3
:pp3 4
$strpp5 D
,ppD E
nullableppF N
:ppN O
falseppP U
)ppU V
,ppV W
BlogIdqq 
=qq 
tableqq "
.qq" #
Columnqq# )
<qq) *
intqq* -
>qq- .
(qq. /
typeqq/ 3
:qq3 4
$strqq5 :
,qq: ;
nullableqq< D
:qqD E
trueqqF J
)qqJ K
}rr 
,rr 
constraintsss 
:ss 
tabless "
=>ss# %
{tt 
tableuu 
.uu 

PrimaryKeyuu $
(uu$ %
$struu% 3
,uu3 4
xuu5 6
=>uu7 9
xuu: ;
.uu; <
IDuu< >
)uu> ?
;uu? @
tablevv 
.vv 

ForeignKeyvv $
(vv$ %
nameww 
:ww 
$strww 9
,ww9 :
columnxx 
:xx 
xxx  !
=>xx" $
xxx% &
.xx& '
BlogIdxx' -
,xx- .
principalTableyy &
:yy& '
$stryy( /
,yy/ 0
principalColumnzz '
:zz' (
$strzz) -
)zz- .
;zz. /
}{{ 
){{ 
;{{ 
migrationBuilder}} 
.}} 
CreateTable}} (
(}}( )
name~~ 
:~~ 
$str~~ 
,~~  
columns 
: 
table 
=> !
new" %
{
�� 
ID
�� 
=
�� 
table
�� 
.
�� 
Column
�� %
<
��% &
int
��& )
>
��) *
(
��* +
type
��+ /
:
��/ 0
$str
��1 6
,
��6 7
nullable
��8 @
:
��@ A
false
��B G
)
��G H
.
�� 

Annotation
�� #
(
��# $
$str
��$ 8
,
��8 9
$str
��: @
)
��@ A
,
��A B
TagName
�� 
=
�� 
table
�� #
.
��# $
Column
��$ *
<
��* +
string
��+ 1
>
��1 2
(
��2 3
type
��3 7
:
��7 8
$str
��9 H
,
��H I
nullable
��J R
:
��R S
false
��T Y
)
��Y Z
,
��Z [
BlogId
�� 
=
�� 
table
�� "
.
��" #
Column
��# )
<
��) *
int
��* -
>
��- .
(
��. /
type
��/ 3
:
��3 4
$str
��5 :
,
��: ;
nullable
��< D
:
��D E
true
��F J
)
��J K
}
�� 
,
�� 
constraints
�� 
:
�� 
table
�� "
=>
��# %
{
�� 
table
�� 
.
�� 

PrimaryKey
�� $
(
��$ %
$str
��% 1
,
��1 2
x
��3 4
=>
��5 7
x
��8 9
.
��9 :
ID
��: <
)
��< =
;
��= >
table
�� 
.
�� 

ForeignKey
�� $
(
��$ %
name
�� 
:
�� 
$str
�� 7
,
��7 8
column
�� 
:
�� 
x
��  !
=>
��" $
x
��% &
.
��& '
BlogId
��' -
,
��- .
principalTable
�� &
:
��& '
$str
��( /
,
��/ 0
principalColumn
�� '
:
��' (
$str
��) -
)
��- .
;
��. /
}
�� 
)
�� 
;
�� 
migrationBuilder
�� 
.
�� 
CreateIndex
�� (
(
��( )
name
�� 
:
�� 
$str
�� '
,
��' (
table
�� 
:
�� 
$str
�� 
,
�� 
column
�� 
:
�� 
$str
��  
)
��  !
;
��! "
migrationBuilder
�� 
.
�� 
CreateIndex
�� (
(
��( )
name
�� 
:
�� 
$str
�� .
,
��. /
table
�� 
:
�� 
$str
�� %
,
��% &
column
�� 
:
�� 
$str
��  
)
��  !
;
��! "
migrationBuilder
�� 
.
�� 
CreateIndex
�� (
(
��( )
name
�� 
:
�� 
$str
�� -
,
��- .
table
�� 
:
�� 
$str
�� $
,
��$ %
column
�� 
:
�� 
$str
��  
)
��  !
;
��! "
migrationBuilder
�� 
.
�� 
CreateIndex
�� (
(
��( )
name
�� 
:
�� 
$str
�� +
,
��+ ,
table
�� 
:
�� 
$str
�� "
,
��" #
column
�� 
:
�� 
$str
��  
)
��  !
;
��! "
migrationBuilder
�� 
.
�� 
CreateIndex
�� (
(
��( )
name
�� 
:
�� 
$str
�� )
,
��) *
table
�� 
:
�� 
$str
��  
,
��  !
column
�� 
:
�� 
$str
��  
)
��  !
;
��! "
}
�� 	
	protected
�� 
override
�� 
void
�� 
Down
��  $
(
��$ %
MigrationBuilder
��% 5
migrationBuilder
��6 F
)
��F G
{
�� 	
migrationBuilder
�� 
.
�� 
	DropTable
�� &
(
��& '
name
�� 
:
�� 
$str
�� $
)
��$ %
;
��% &
migrationBuilder
�� 
.
�� 
	DropTable
�� &
(
��& '
name
�� 
:
�� 
$str
�� #
)
��# $
;
��$ %
migrationBuilder
�� 
.
�� 
	DropTable
�� &
(
��& '
name
�� 
:
�� 
$str
�� !
)
��! "
;
��" #
migrationBuilder
�� 
.
�� 
	DropTable
�� &
(
��& '
name
�� 
:
�� 
$str
�� 
)
��  
;
��  !
migrationBuilder
�� 
.
�� 
	DropTable
�� &
(
��& '
name
�� 
:
�� 
$str
�� 
)
�� 
;
�� 
migrationBuilder
�� 
.
�� 
	DropTable
�� &
(
��& '
name
�� 
:
�� 
$str
�� 
)
�� 
;
�� 
}
�� 	
}
�� 
}�� �
TD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Mappers\ProblemMapper.cs
	namespace		 	
api		
 
.		 
Mappers		 
{

 
public 

static 
class 
ProblemMapper %
{ 
public 
static 
ViewAllProblemDto '
ToViewAllProblemDto( ;
(; <
Problem< C
problemD K
,K L
ListM Q
<Q R

SubmissionR \
>\ ]
submissions^ i
,i j
stringk q
userIdr x
)x y
{ 	
int 
totalSubmissions  
=! "
submissions# .
.. /
Count/ 4
;4 5
int 
acceptedSubmissions #
=$ %
submissions& 1
.1 2
Count2 7
(7 8
s8 9
=>: <
s= >
.> ?
Status? E
==F H
$strI S
)S T
;T U
var 

problemDto 
= 
new  
ViewAllProblemDto! 2
{ 
	ProblemId 
= 
problem #
.# $
	ProblemId$ -
,- .
Title 
= 
problem 
.  
Title  %
,% &
Category 
= 
problem "
." #
Category# +
??, .
$str/ >
,> ?
Score 
= 
problem 
.  

TotalPoint  *
,* +
AcceptanceRate 
=  
totalSubmissions! 1
>2 3
$num4 5
?6 7
(8 9
double9 ?
)? @
acceptedSubmissions@ S
/T U
totalSubmissionsV f
*g h
$numi l
:m n
$numo p
,p q
AcceptedCount 
= 
acceptedSubmissions  3
,3 4
SolvedStatus 
= 
submissions *
.* +
Any+ .
(. /
s/ 0
=>1 3
s4 5
.5 6
AppUser6 =
.= >
Id> @
.@ A
EqualsA G
(G H
userIdH N
)N O
&&P R
sS T
.T U
StatusU [
==\ ^
$str_ i
)i j
?k l
$strm u
:v w
$str	x �
} 
; 
return 

problemDto 
; 
} 	
}   
}!! �%
\D:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Mappers\ProblemHomePageMapper.cs
	namespace 	
api
 
. 
Mappers 
{		 
public

 

static

 
class

 !
ProblemHomePageMapper

 -
{ 
public 
static *
ProblemHomePageMostAttempedDto 47
+ToProblemHomePageMostAttempedDtoFromProblem5 `
(` a
thisa e
Problemf m
problemn u
)u v
{ 	
return 
new *
ProblemHomePageMostAttempedDto 5
{ 
	ProblemId 
= 
problem #
.# $
	ProblemId$ -
,- .
Category 
= 
problem "
." #
Category# +
,+ ,
Title 
= 
problem 
.  
Title  %
,% &
Detail 
= 
problem  
.  !
Detail! '
,' (

TotalPoint 
= 
problem $
.$ %

TotalPoint% /
,/ 0
	TimeLimit 
= 
problem #
.# $
	TimeLimit$ -
,- .
MemoryLimit 
= 
problem %
.% &
MemoryLimit& 1
,1 2
Author 
= 
problem  
.  !
Author! '
,' (
} 
; 
} 	
public 
static *
ProblemHomePageMostAttempedDto 4
AddNumAttempted5 D
(D E
thisE I*
ProblemHomePageMostAttempedDtoJ h
problemi p
,p q
(r s
ints v
numAttempted	w �
,
� �
int
� �
	numSucces
� �
)
� �
data
� �
)
� �
{ 	
return 
new *
ProblemHomePageMostAttempedDto 5
{ 
	ProblemId 
= 
problem #
.# $
	ProblemId$ -
,- .
Category   
=   
problem   "
.  " #
Category  # +
,  + ,
Title!! 
=!! 
problem!! 
.!!  
Title!!  %
,!!% &
Detail"" 
="" 
problem""  
.""  !
Detail""! '
,""' (

TotalPoint## 
=## 
problem## $
.##$ %

TotalPoint##% /
,##/ 0
	TimeLimit$$ 
=$$ 
problem$$ #
.$$# $
	TimeLimit$$$ -
,$$- .
MemoryLimit%% 
=%% 
problem%% %
.%%% &
MemoryLimit%%& 1
,%%1 2
Author&& 
=&& 
problem&&  
.&&  !
Author&&! '
,&&' (
TimeAttempted'' 
='' 
data''  $
.''$ %
numAttempted''% 1
,''1 2
	NumSucces(( 
=(( 
data((  
.((  !
	numSucces((! *
})) 
;)) 
}** 	
public,, 
static,, %
ProblemHomePageNotDoneDto,, //
#ToProblemHomePageNotDoneFromProblem,,0 S
(,,S T
this,,T X
Problem,,Y `
problem,,a h
,,,h i
(,,j k
int,,k n
point,,o t
,,,t u
string,,v |
status	,,} �
)
,,� �
data
,,� �
)
,,� �
{-- 	
return.. 
new.. %
ProblemHomePageNotDoneDto.. 0
{// 
	ProblemId00 
=00 
problem00 #
.00# $
	ProblemId00$ -
,00- .
Category11 
=11 
problem11 "
.11" #
Category11# +
,11+ ,
Title22 
=22 
problem22 
.22  
Title22  %
,22% &
Detail33 
=33 
problem33  
.33  !
Detail33! '
,33' (

TotalPoint44 
=44 
problem44 $
.44$ %

TotalPoint44% /
,44/ 0
	TimeLimit55 
=55 
problem55 #
.55# $
	TimeLimit55$ -
,55- .
MemoryLimit66 
=66 
problem66 %
.66% &
MemoryLimit66& 1
,661 2
Author77 
=77 
problem77  
.77  !
Author77! '
,77' (
Point88 
=88 
data88 
.88 
point88 "
,88" #
Status99 
=99 
data99 
.99 
status99 $
}:: 
;:: 
};; 	
}<< 
}== �
QD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Mappers\BlogMapper.cs
	namespace		 	
api		
 
.		 
Mappers		 
{

 
public 

static 
class 

BlogMapper "
{ 
public 
static 
BlogDto 
	ToBlogDto '
(' (
this( ,
Blog- 1
	BlogModel2 ;
); <
{ 	
return 
new 
BlogDto 
{ 
ID 
= 
	BlogModel 
. 
ID !
,! "
UserId 
= 
	BlogModel "
." #
UserId# )
,) *
	GuestName 
= 
	BlogModel %
.% &
	GuestName& /
,/ 0

GuestEmail 
= 
	BlogModel &
.& '

GuestEmail' 1
,1 2
	Thumbnail 
= 
	BlogModel %
.% &
	Thumbnail& /
,/ 0
title 
= 
	BlogModel !
.! "
title" '
,' (
description 
= 
	BlogModel '
.' (
description( 3
,3 4
Content 
= 
	BlogModel #
.# $
Content$ +
,+ ,
	ImageBlog 
= 
	BlogModel %
.% &
	ImageBlog& /
,/ 0
CategoryBlog 
= 
	BlogModel (
.( )
CategoryBlog) 5
,5 6
Status 
= 
	BlogModel "
." #
Status# )
,) *
CreateOn   
=   
	BlogModel   $
.  $ %
CreateOn  % -
,  - .

DatePublic!! 
=!! 
	BlogModel!! &
.!!& '

DatePublic!!' 1
,!!1 2
TagBlog"" 
="" 
	BlogModel"" #
.""# $
TagBlog""$ +
}## 
;## 
}$$ 	
public%% 
static%% 
Blog%% 
ToBlogFromCreateDto%% .
(%%. /
this%%/ 3 
CreateBlogRequestDto%%4 H
BlogDto%%I P
)%%P Q
{&& 	
return'' 
new'' 
Blog'' 
{(( 
	GuestName** 
=** 
BlogDto** #
.**# $
	GuestName**$ -
,**- .

GuestEmail++ 
=++ 
BlogDto++ $
.++$ %

GuestEmail++% /
,++/ 0
	Thumbnail,, 
=,, 
BlogDto,, #
.,,# $
	Thumbnail,,$ -
,,,- .
title-- 
=-- 
BlogDto-- 
.--  
title--  %
,--% &
description.. 
=.. 
BlogDto.. %
...% &
description..& 1
,..1 2
Content// 
=// 
BlogDto// !
.//! "
Content//" )
,//) *
Status00 
=00 
BlogDto00  
.00  !
Status00! '
,00' (
CreateOn11 
=11 
BlogDto11 "
.11" #
CreateOn11# +
,11+ ,

DatePublic22 
=22 
BlogDto22 $
.22$ %

DatePublic22% /
,22/ 0
}33 
;33 
}44 	
}55 
}66 �
VD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Interface\ITokenService.cs
	namespace 	
api
 
. 
	Interface 
{ 
public		 

	interface		 
ITokenService		 "
{

 
string 
CreateToken 
( 
AppUser "
user# '
)' (
;( )
} 
} �
^D:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Interface\ISubmissionRepository.cs
	namespace 	
api
 
. 
	Interface 
{ 
public		 

	interface		 !
ISubmissionRepository		 *
{

 
public 
Task 
< 
List 
< 

Submission #
># $
>$ %*
GetSubmissionsByProblemIdAsync& D
(D E
stringE K
	problemIdL U
)U V
;V W
public 
Task 
< 
List 
< 

Submission #
?# $
>$ %
>% &(
GetAllSubmissionAtMonthAsync' C
(C D
intD G
monthH M
,M N
intO R
yearS W
)W X
;X Y
} 
} �
XD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Interface\IProblemService.cs
	namespace

 	
api


 
.

 
	Interface

 
{ 
public 

	interface 
IProblemService $
{ 
Task 
< 

PageResult 
< 
ViewAllProblemDto )
>) *
>* +(
GetAllProblemsWithStatsAsync, H
(H I
stringI O
userIdP V
,V W
QueryObjectX c
queryd i
)i j
;j k
Task 
< 
List 
< 
string 
> 
> !
GetAllCategoriesAsync 0
(0 1
)1 2
;2 3
} 
} �
[D:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Interface\IProblemRepository.cs
	namespace 	
api
 
. 
	Interface 
{ 
public		 

	interface		 
IProblemRepository		 '
{

 
public 
Task 
< 
List 
< 
Problem  
>  !
>! "
GetAllProblemAsync# 5
(5 6
)6 7
;7 8
} 
} �	
aD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Interface\IProblemHomePageServices.cs
	namespace 	
api
 
. 
	Interface 
{ 
public		 

	interface		 $
IProblemHomePageServices		 -
{

 
public 
Task 
< 
List 
< *
ProblemHomePageMostAttempedDto 7
?7 8
>8 9
>9 :/
#GetXProblemHomePageMostAttmpedAsync; ^
(^ _
int_ b
pageSizec k
,k l
intm p
monthq v
,v w
intx {
year	| �
)
� �
;
� �
public 
Task 
< 
List 
< %
ProblemHomePageNotDoneDto 2
?2 3
>3 4
>4 5&
GetXProblemAreNotDoneAsync6 P
(P Q
intQ T
pageSizeU ]
,] ^
string_ e
userIdf l
,l m
intn q
monthr w
,w x
inty |
year	} �
)
� �
;
� �
} 
} �
cD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Interface\IProblemHomePageRepository.cs
	namespace 	
api
 
. 
	Interface 
{ 
public		 

	interface		 &
IProblemHomePageRepository		 /
{

 
} 
} �
XD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Interface\IBlogRepository.cs
	namespace		 	
api		
 
.		 
	Interface		 
{

 
public 

	interface 
IBlogRepository $
{ 
Task 
< 
List 
< 
Blog 
> 
> 
GetAllAsync $
($ %
)% &
;& '
Task 
< 
Blog 
? 
> 
GetByIDAsync  
(  !
int! $
id% '
)' (
;( )
Task 
< 
List 
< 
Blog 
> 
> 
GetByUserIDAsync )
() *
string* 0
id1 3
)3 4
;4 5
Task 
< 
Blog 
> 
CreateAsync 
( 
Blog #
	BlogModel$ -
)- .
;. /
Task 
< 
Blog 
? 
> 

UpdateAync 
( 
int "
id# %
,% &
UpdateBlogRequesDto' :
BlogDto; B
)B C
;C D
Task 
< 
Blog 
? 
> 

DeleteAync 
( 
int "
id# %
)% &
;& '
} 
} �
RD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Helpers\QueryObject.cs
	namespace 	
api
 
. 
Helpers 
{ 
public 

class 
QueryObject 
{		 
public

 
string

 
ProblemTitle

 "
{

# $
get

% (
;

( )
set

* -
;

- .
}

/ 0
=

1 2
string

3 9
.

9 :
Empty

: ?
;

? @
public 
string 
ProblemCategory %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
=4 5
string6 <
.< =
Empty= B
;B C
public 
string 

HidePassed  
{! "
get# &
;& '
set( +
;+ ,
}- .
=/ 0
string1 7
.7 8
Empty8 =
;= >
public 
string 
SortBy 
{ 
get "
;" #
set$ '
;' (
}) *
=+ ,
string- 3
.3 4
Empty4 9
;9 :
public 
string 
IsDescending "
{# $
get% (
;( )
set* -
;- .
}/ 0
=1 2
$str3 :
;: ;
public 
int 

PageNumber 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
$num. /
;/ 0
public 
int 
PageSize 
{ 
get !
;! "
set# &
;& '
}( )
=* +
$num, .
;. /
} 
} �
aD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Helpers\ProblemHomePageQueryObject.cs
	namespace 	
api
 
. 
Helpers 
{ 
public 

class &
ProblemHomePageQueryObject +
{		 
public 
bool 
MostAttemped  
{! "
get# &
;& '
set( +
;+ ,
}- .
=/ 0
false1 6
;6 7
public 
bool 
NotDone 
{ 
get !
;! "
set# &
;& '
}( )
=* +
false, 1
;1 2
public 
bool 
Done 
{ 
get 
; 
set  #
;# $
}% &
=' (
false) .
;. /
public 
string 
userId 
{ 
get "
;" #
set$ '
;' (
}) *
=+ ,
string- 3
.3 4
Empty4 9
;9 :
public 
int 
PageSize 
{ 
get !
;! "
set# &
;& '
}( )
=* +
$num, -
;- .
public 
int 

PageNumber 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
$num. /
;/ 0
} 
} �
mD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Dtos\ProblemHomePage\ProblemHomePageNotDoneDto.cs
	namespace 	
api
 
. 
Dtos 
. 
ProblemHomePage "
{ 
public 

class %
ProblemHomePageNotDoneDto *
{		 
public

 
string

 
	ProblemId

 
{

  !
get

" %
;

% &
set

' *
;

* +
}

, -
=

. /
string

0 6
.

6 7
Empty

7 <
;

< =
public 
string 
Category 
{  
get! $
;$ %
set& )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
public 
string 
Title 
{ 
get !
;! "
set# &
;& '
}( )
=* +
string, 2
.2 3
Empty3 8
;8 9
public 
string 
Detail 
{ 
get "
;" #
set$ '
;' (
}) *
=+ ,
string- 3
.3 4
Empty4 9
;9 :
public 
int 

TotalPoint 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
$num. /
;/ 0
public 
int 
	TimeLimit 
{ 
get "
;" #
set$ '
;' (
}) *
public 
int 
MemoryLimit 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
string 
? 
Author 
{ 
get  #
;# $
set% (
;( )
}* +
public 
float 
Point 
{ 
get  
;  !
set" %
;% &
}' (
public 
string 
Status 
{ 
get "
;" #
set$ '
;' (
}) *
=+ ,
string- 3
.3 4
Empty4 9
;9 :
public 
override 
bool 
Equals #
(# $
object$ *
obj+ .
). /
{ 	
return 
obj 
is %
ProblemHomePageNotDoneDto 3
dto4 7
&&8 :
	ProblemId 
== 
dto  #
.# $
	ProblemId$ -
;- .
} 	
public 
override 
int 
GetHashCode '
(' (
)( )
{ 	
return 
	ProblemId 
. 
GetHashCode (
(( )
)) *
;* +
} 	
} 
}   �
]D:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Dtos\Problem\ViewAllProblemDto.cs
	namespace 	
api
 
. 
Dtos 
. 
Problem 
{ 
public 

class 
ViewAllProblemDto "
{		 
public

 
string

 
	ProblemId

 
{

  !
get

" %
;

% &
set

' *
;

* +
}

, -
=

. /
string

0 6
.

6 7
Empty

7 <
;

< =
public 
string 
Title 
{ 
get !
;! "
set# &
;& '
}( )
=* +
string, 2
.2 3
Empty3 8
;8 9
public 
string 
Category 
{  
get! $
;$ %
set& )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
public 
double 
Score 
{ 
get !
;! "
set# &
;& '
}( )
public 
double 
AcceptanceRate $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 
int 
AcceptedCount  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 
string 
SolvedStatus "
{# $
get% (
;( )
set* -
;- .
}/ 0
=1 2
string3 9
.9 :
Empty: ?
;? @
} 
} �
rD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Dtos\ProblemHomePage\ProblemHomePageMostAttempedDto.cs
	namespace 	
api
 
. 
Dtos 
. 
ProblemHomePage "
{ 
public 

class *
ProblemHomePageMostAttempedDto /
{		 
public

 
string

 
	ProblemId

 
{

  !
get

" %
;

% &
set

' *
;

* +
}

, -
=

. /
string

0 6
.

6 7
Empty

7 <
;

< =
public 
string 
Category 
{  
get! $
;$ %
set& )
;) *
}+ ,
=- .
string/ 5
.5 6
Empty6 ;
;; <
public 
string 
Title 
{ 
get !
;! "
set# &
;& '
}( )
=* +
string, 2
.2 3
Empty3 8
;8 9
public 
string 
Detail 
{ 
get "
;" #
set$ '
;' (
}) *
=+ ,
string- 3
.3 4
Empty4 9
;9 :
public 
int 

TotalPoint 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
$num. /
;/ 0
public 
int 
	TimeLimit 
{ 
get "
;" #
set$ '
;' (
}) *
public 
int 
MemoryLimit 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
string 
? 
Author 
{ 
get  #
;# $
set% (
;( )
}* +
public 
int 
TimeAttempted  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 
int 
	NumSucces 
{ 
get "
;" #
set$ '
;' (
}) *
public 
override 
bool 
Equals #
(# $
object$ *
obj+ .
). /
{ 	
return 
obj 
is *
ProblemHomePageMostAttempedDto 8
dto9 <
&&= ?
	ProblemId 
== 
dto  #
.# $
	ProblemId$ -
;- .
} 	
public 
override 
int 
GetHashCode '
(' (
)( )
{ 	
return 
	ProblemId 
. 
GetHashCode (
(( )
)) *
;* +
} 	
} 
}   �
aD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Dtos\BlogSpace\UpdateBlogRequesDto.cs
	namespace 	
api
 
. 
Dtos 
. 
	BlogSpace 
{ 
public 

class 
UpdateBlogRequesDto $
{		 
public

 
string

 
	Thumbnail

 
{

  !
get

" %
;

% &
set

' *
;

* +
}

, -
=

. /
string

0 6
.

6 7
Empty

7 <
;

< =
public 
string 
title 
{ 
get !
;! "
set# &
;& '
}( )
=* +
string, 2
.2 3
Empty3 8
;8 9
public 
string 
description !
{" #
get$ '
;' (
set) ,
;, -
}. /
=0 1
string2 8
.8 9
Empty9 >
;> ?
public 
string 
Content 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
string. 4
.4 5
Empty5 :
;: ;
public 
string 
Status 
{ 
get "
;" #
set$ '
;' (
}) *
=+ ,
string- 3
.3 4
Empty4 9
;9 :
} 
} �
bD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Dtos\BlogSpace\CreateBlogRequestDto.cs
	namespace 	
api
 
. 
Dtos 
{ 
public		 

class		  
CreateBlogRequestDto		 %
{

 
public 
string 
? 
	GuestName  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 
string 
? 

GuestEmail !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
string 
	Thumbnail 
{  !
get" %
;% &
set' *
;* +
}, -
=. /
string0 6
.6 7
Empty7 <
;< =
public 
string 
title 
{ 
get !
;! "
set# &
;& '
}( )
=* +
string, 2
.2 3
Empty3 8
;8 9
public 
string 
description !
{" #
get$ '
;' (
set) ,
;, -
}. /
=0 1
string2 8
.8 9
Empty9 >
;> ?
public 
string 
Content 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
string. 4
.4 5
Empty5 :
;: ;
public 
string 
Status 
{ 
get "
;" #
set$ '
;' (
}) *
=+ ,
string- 3
.3 4
Empty4 9
;9 :
public 
DateTime 
CreateOn  
{! "
get# &
;& '
set( +
;+ ,
}- .
=/ 0
DateTime1 9
.9 :
Now: =
;= >
public 
DateTime 
? 

DatePublic #
{$ %
get& )
;) *
set+ .
;. /
}0 1
} 
} �
UD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Dtos\BlogSpace\BlogDto.cs
	namespace 	
api
 
. 
Dtos 
. 
	BlogSpace 
{ 
public		 

class		 
BlogDto		 
{

 
public 
int 
ID 
{ 
get 
; 
set  
;  !
}" #
public 
string 
? 
UserId 
{ 
get  #
;# $
set% (
;( )
}* +
public 
string 
? 
	GuestName  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 
string 
? 

GuestEmail !
{" #
get$ '
;' (
set) ,
;, -
}. /
public 
string 
	Thumbnail 
{  !
get" %
;% &
set' *
;* +
}, -
=. /
string0 6
.6 7
Empty7 <
;< =
public 
string 
title 
{ 
get !
;! "
set# &
;& '
}( )
=* +
string, 2
.2 3
Empty3 8
;8 9
public 
string 
description !
{" #
get$ '
;' (
set) ,
;, -
}. /
=0 1
string2 8
.8 9
Empty9 >
;> ?
public 
string 
Content 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
string. 4
.4 5
Empty5 :
;: ;
public 
string 
	ImageBlog 
{  !
get" %
;% &
set' *
;* +
}, -
=. /
string0 6
.6 7
Empty7 <
;< =
public 
string 
CategoryBlog "
{# $
get% (
;( )
set* -
;- .
}/ 0
=1 2
string3 9
.9 :
Empty: ?
;? @
public 
string 
Status 
{ 
get "
;" #
set$ '
;' (
}) *
=+ ,
string- 3
.3 4
Empty4 9
;9 :
public 
DateTime 
CreateOn  
{! "
get# &
;& '
set( +
;+ ,
}- .
=/ 0
DateTime1 9
.9 :
Now: =
;= >
public 
DateTime 
? 

DatePublic #
{$ %
get& )
;) *
set+ .
;. /
}0 1
public 
string 
TagBlog 
{ 
get  #
;# $
set% (
;( )
}* +
=, -
string. 4
.4 5
Empty5 :
;: ;
} 
}   �
WD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Dtos\Account\RegisterDto.cs
	namespace 	
api
 
. 
Dtos 
. 
Account 
{ 
public 

class 
RegisterDto 
{ 
[ 	
Required	 
] 
public		 
string		 
Username		 
{		  
get		! $
;		$ %
set		& )
;		) *
}		+ ,
[ 	
Required	 
] 
[ 	
EmailAddress	 
] 
public 
string 
Email 
{ 
get !
;! "
set# &
;& '
}( )
[ 	
Required	 
] 
public 
string 
Password 
{  
get! $
;$ %
set& )
;) *
}+ ,
[ 	
Required	 
] 
public 
string  
?  !
FullName" *
{+ ,
get- 0
;0 1
set2 5
;5 6
}7 8
[ 	
Required	 
] 
public 
string  
?  !
Address" )
{* +
get, /
;/ 0
set1 4
;4 5
}6 7
[ 	
Required	 
] 
public 
string  
PhoneNumber! ,
{- .
get/ 2
;2 3
set4 7
;7 8
}9 :
[ 	
Required	 
] 
public 
DateTime "
?" #
DateOfBirth$ /
{0 1
get2 5
;5 6
set7 :
;: ;
}< =
} 
} �
VD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Dtos\Account\NewUserDto.cs
	namespace 	
api
 
. 
Dto 
. 
Account 
{ 
public 

class 

NewUserDto 
{		 
public

 
string

 
Id

 
{

 
get

 
;

 
set

  #
;

# $
}

% &
public 
string 
UserName 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 
string 
Email 
{ 
get !
;! "
set# &
;& '
}( )
public 
DateTime 
? 
DateOfBirth $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 
string 
Token 
{ 
get !
;! "
set# &
;& '
}( )
} 
} �
TD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Dtos\Account\LoginDto.cs
	namespace 	
api
 
. 
Dto 
. 
Account 
{ 
public		 

class		 
LoginDto		 
{

 
[ 
Required 
] 
public 
string 
Username %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
[ 
Required 
] 
public 
string 
Password %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
} 
} �(
XD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Data\ApplicationDBContext.cs
	namespace

 	
api


 
.

 
Data

 
{ 
public 

class  
ApplicationDBContext %
:& '
IdentityDbContext( 9
<9 :
AppUser: A
>A B
{ 
public  
ApplicationDBContext #
(# $
DbContextOptions$ 4
<4 5 
ApplicationDBContext5 I
>I J
optionsK R
)R S
:T U
baseV Z
(Z [
options[ b
)b c
{ 	
} 	
public 
DbSet 
< 
Problem 
> 
Problems &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
public 
DbSet 
< 

Submission 
>  
Submissions! ,
{- .
get/ 2
;2 3
set4 7
;7 8
}9 :
public 
DbSet 
< 
AppUser 
> 
AppUsers &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
public 
DbSet 
< 
ContestRegistion %
>% &
ContestRegistions' 8
{9 :
get; >
;> ?
set@ C
;C D
}E F
public 
DbSet 
< 
TestCase 
> 
	TestCases (
{) *
get+ .
;. /
set0 3
;3 4
}5 6
public 
DbSet 
< 
TestCaseStatus #
># $
TestCaseStatuses% 5
{6 7
get8 ;
;; <
set= @
;@ A
}B C
public 
DbSet 
< 
ContestProblem #
># $
ContestProblems% 4
{5 6
get7 :
;: ;
set< ?
;? @
}A B
public 
DbSet 
< 
Contest 
> 
Contests &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
public 
DbSet 
< 
Blog 
> 
Blogs  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 
DbSet 
< 
CommentBlog  
>  !
CommentBlog" -
{. /
get0 3
;3 4
set5 8
;8 9
}: ;
	protected 
override 
void 
OnModelCreating  /
(/ 0
ModelBuilder0 <
modelBuilder= I
)I J
{   	
base!! 
.!! 
OnModelCreating!!  
(!!  !
modelBuilder!!! -
)!!- .
;!!. /
modelBuilder$$ 
.$$ 
Entity$$ 
<$$  
ContestProblem$$  .
>$$. /
($$/ 0
entity$$0 6
=>$$7 9
{%% 
entity&& 
.&& 
HasKey&& 
(&& 
cp&&  
=>&&! #
new&&$ '
{&&( )
cp&&* ,
.&&, -
	ContestId&&- 6
,&&6 7
cp&&8 :
.&&: ;
	ProblemId&&; D
}&&E F
)&&F G
;&&G H
}'' 
)'' 
;'' 
modelBuilder** 
.** 
Entity** 
<**  
ContestRegistion**  0
>**0 1
(**1 2
)**2 3
.++ 
HasKey++ 
(++ 
cr++ 
=>++ 
new++ !
{++" #
cr++$ &
.++& '
Id++' )
,++) *
cr+++ -
.++- .
	ContestId++. 7
}++8 9
)++9 :
;++: ;
List.. 
<.. 
IdentityRole.. 
>.. 
roles.. $
=..% &
new..' *
List..+ /
<../ 0
IdentityRole..0 <
>..< =
{// 
new00 
IdentityRole00 
{11 	
Name22 
=22 
$str22 
,22 
NormalizedName33 
=33 
$str33 #
}44 	
,44	 

new55 
IdentityRole55 
{66 	
Name77 
=77 
$str77 
,77 
NormalizedName88 
=88 
$str88 $
}99 	
,99	 

new:: 
IdentityRole:: 
{;; 	
Name<< 
=<< 
$str<< 
,<< 
NormalizedName== 
=== 
$str== $
}>> 	
}?? 
;?? 
modelBuilderAA 
.AA 
EntityAA 
<AA  
IdentityRoleAA  ,
>AA, -
(AA- .
)AA. /
.AA/ 0
HasDataAA0 7
(AA7 8
rolesAA8 =
)AA= >
;AA> ?
}BB 	
}EE 
}FF �
dD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Controllers\ProblemHomePageController.cs
	namespace

 	
api


 
.

 
Controllers

 
{ 
[ 
Route 

(
 
$str 
) 
] 
[ 
ApiController 
] 
public 

class %
ProblemHomePageController *
:+ ,
ControllerBase- ;
{ 
private 
readonly $
IProblemHomePageServices 1
_proService2 =
;= >
public %
ProblemHomePageController (
(( )$
IProblemHomePageServices) A

proServiceB L
)L M
{ 	
_proService 
= 

proService $
;$ %
} 	
[ 	
HttpGet	 
] 
public 
async 
Task 
< 
IActionResult '
>' (
getXProblems) 5
(5 6
[6 7
	FromQuery7 @
]@ A&
ProblemHomePageQueryObjectB \
query] b
)b c
{ 	
if 
( 
query 
. 
MostAttemped "
==# %
true& *
)* +
{ 
var 
problems 
= 
await $
_proService% 0
.0 1/
#GetXProblemHomePageMostAttmpedAsync1 T
(T U
queryU Z
.Z [
PageSize[ c
,c d
DateTimee m
.m n
Nown q
.q r
Monthr w
,w x
DateTime	y �
.
� �
Now
� �
.
� �
Year
� �
)
� �
;
� �
if 
( 
problems 
== 
null  $
)$ %
return 
NotFound #
(# $
$str$ I
)I J
;J K
return   
Ok   
(   
problems   "
)  " #
;  # $
}!! 
else"" 
if"" 
("" 
query"" 
."" 
NotDone"" "
==""# %
true""& *
)""* +
{## 
var$$ 
problems$$ 
=$$ 
await$$ $
_proService$$% 0
.$$0 1&
GetXProblemAreNotDoneAsync$$1 K
($$K L
query$$L Q
.$$Q R
PageSize$$R Z
,$$Z [
query$$\ a
.$$a b
userId$$b h
,$$h i
DateTime$$j r
.$$r s
Now$$s v
.$$v w
Month$$w |
,$$| }
DateTime	$$~ �
.
$$� �
Now
$$� �
.
$$� �
Year
$$� �
)
$$� �
;
$$� �
if&& 
(&& 
problems&& 
==&& 
null&&  $
)&&$ %
return'' 
NotFound'' #
(''# $
$str''$ I
)''I J
;''J K
return)) 
Ok)) 
()) 
problems)) "
)))" #
;))# $
}** 
return,, 

BadRequest,, 
(,, 
),, 
;,,  
}-- 	
}.. 
}// �!
\D:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Controllers\ProblemController.cs
	namespace

 	
api


 
.

 
Controllers

 
{ 
[ 
Route 

(
 
$str 
) 
] 
[ 
ApiController 
] 
public 

class 
ProblemController "
:# $
ControllerBase% 3
{ 
private 
readonly 
IProblemService (
_problemService) 8
;8 9
public 
ProblemController  
(  !
IProblemService! 0
problemService1 ?
)? @
{ 	
_problemService 
= 
problemService ,
;, -
} 	
[ 	
HttpGet	 
] 
public 
async 
Task 
< 
IActionResult '
>' (&
GetAllProblemsForUserAsync) C
(C D
[D E
	FromQueryE N
]N O
QueryObjectP [
query\ a
)a b
{ 	
var 
userId 
= 
HttpContext $
.$ %
User% )
?) *
.* +
Claims+ 1
.1 2
FirstOrDefault2 @
(@ A
cA B
=>C E
cF G
.G H
TypeH L
==M O
$strP X
)X Y
?Y Z
.Z [
Value[ `
;` a
if 
( 
string 
. 
IsNullOrEmpty $
($ %
userId% +
)+ ,
), -
{ 
userId 
= 
string 
.  
Empty  %
;% &
} 
var 

categories 
= 
await "
_problemService# 2
.2 3!
GetAllCategoriesAsync3 H
(H I
)I J
;J K
var   
result   
=   
await   
_problemService   .
.  . /(
GetAllProblemsWithStatsAsync  / K
(  K L
userId  L R
,  R S
query  T Y
)  Y Z
;  Z [
if!! 
(!! 
query!! 
.!! 

PageNumber!!  
>!!! "
result!!# )
.!!) *

TotalPages!!* 4
)!!4 5
{"" 
return## 
NotFound## 
(##  
)##  !
;##! "
}$$ 
if%% 
(%% 
string%% 
.%% 
IsNullOrWhiteSpace%% )
(%%) *
query%%* /
.%%/ 0
IsDescending%%0 <
)%%< =
||%%> @
(&& 
query&& 
.&& 
IsDescending&& #
!=&&$ &
$str&&' -
&&&&. 0
query&&1 6
.&&6 7
IsDescending&&7 C
!=&&D F
$str&&G N
)&&N O
)&&O P
{'' 
return(( 
NotFound(( 
(((  
)((  !
;((! "
})) 
if** 
(** 
!** 

categories** 
.** 
Contains** $
(**$ %
query**% *
.*** +
ProblemCategory**+ :
)**: ;
)**; <
{++ 
return,, 
Ok,, 
(,, 
result,,  
),,  !
;,,! "
}-- 
return.. 
Ok.. 
(.. 
result.. 
).. 
;.. 
}// 	
[11 	
HttpGet11	 
(11 
$str11 
)11 
]11 
public22 
async22 
Task22 
<22 
IActionResult22 '
>22' (
GetCategories22) 6
(226 7
)227 8
{33 	
var44 

categories44 
=44 
await44 "
_problemService44# 2
.442 3!
GetAllCategoriesAsync443 H
(44H I
)44I J
;44J K
return55 
Ok55 
(55 

categories55  
)55  !
;55! "
}66 	
}77 
}88 �5
YD:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Controllers\BlogController.cs
	namespace 	
api
 
. 
Controllers 
{ 
[ 
Route 

(
 
$str 
)  
]  !
[ 
ApiController 
] 
public 

class 
BlogController 
:  !
ControllerBase" 0
{ 
private 
readonly 
IBlogRepository (
	_BlogRepo) 2
;2 3
public 
BlogController 
( 
IBlogRepository -
blogRepo. 6
)6 7
{ 	
	_BlogRepo 
= 
blogRepo  
;  !
} 	
[ 	
HttpGet	 
] 
public 
async 
Task 
< 
IActionResult '
>' (
GetAll) /
(/ 0
)0 1
{ 	
var 
blogs 
= 
await 
	_BlogRepo '
.' (
GetAllAsync( 3
(3 4
)4 5
;5 6
var 
blogsDto 
= 
blogs  
.  !
Select! '
(' (
s( )
=>* ,
s- .
.. /
	ToBlogDto/ 8
(8 9
)9 :
): ;
;; <
return 
Ok 
( 
blogsDto 
) 
;  
}   	
[!! 	
HttpGet!!	 
(!! 
$str!! 
)!! 
]!! 
public"" 
async"" 
Task"" 
<"" 
IActionResult"" '
>""' (
GetByID"") 0
(""0 1
[""1 2
	FromRoute""2 ;
]""; <
int""= @
id""A C
)""C D
{## 	
var$$ 
stock$$ 
=$$ 
await$$ 
	_BlogRepo$$ '
.$$' (
GetByIDAsync$$( 4
($$4 5
id$$5 7
)$$7 8
;$$8 9
if&& 
(&& 
stock&& 
==&& 
null&& 
)&& 
{'' 
return(( 
NotFound(( 
(((  
)((  !
;((! "
})) 
return++ 
Ok++ 
(++ 
stock++ 
.++ 
	ToBlogDto++ %
(++% &
)++& '
)++' (
;++( )
},, 	
[-- 	
HttpGet--	 
(-- 
$str--  
)--  !
]--! "
public.. 
async.. 
Task.. 
<.. 
IActionResult.. '
>..' (
GetByUserID..) 4
(..4 5
[..5 6
	FromRoute..6 ?
]..? @
string..A G
userId..H N
)..N O
{// 	
var00 
blogs00 
=00 
await00 
	_BlogRepo00 '
.00' (
GetByUserIDAsync00( 8
(008 9
userId009 ?
)00? @
;00@ A
var11 
blogsDto11 
=11 
blogs11  
.11  !
Select11! '
(11' (
s11( )
=>11* ,
s11- .
.11. /
	ToBlogDto11/ 8
(118 9
)119 :
)11: ;
;11; <
return33 
Ok33 
(33 
blogsDto33 
)33 
;33  
}44 	
[55 	
HttpPost55	 
]55 
public66 
async66 
Task66 
<66 
IActionResult66 '
>66' (
create66) /
(66/ 0
[660 1
FromBody661 9
]669 : 
CreateBlogRequestDto66; O
BlogDto66P W
)66W X
{77 	
var99 
	BlogModel99 
=99 
BlogDto99 #
.99# $
ToBlogFromCreateDto99$ 7
(997 8
)998 9
;999 :
await:: 
	_BlogRepo:: 
.:: 
CreateAsync:: '
(::' (
	BlogModel::( 1
)::1 2
;::2 3
return;; 
CreatedAtAction;; "
(;;" #
nameof;;# )
(;;) *
GetByID;;* 1
);;1 2
,;;2 3
new;;4 7
{;;8 9
id;;: <
=;;= >
	BlogModel;;? H
.;;H I
ID;;I K
};;L M
,;;M N
	BlogModel;;O X
.;;X Y
	ToBlogDto;;Y b
(;;b c
);;c d
);;d e
;;;e f
}<< 	
[== 	
HttpPut==	 
]== 
[>> 	
Route>>	 
(>> 
$str>> 
)>> 
]>> 
public@@ 
async@@ 
Task@@ 
<@@ 
IActionResult@@ '
>@@' (
Update@@) /
(@@/ 0
[@@0 1
	FromRoute@@1 :
]@@: ;
int@@< ?
id@@@ B
,@@B C
[@@D E
FromBody@@E M
]@@M N
UpdateBlogRequesDto@@O b
	updateDto@@c l
)@@l m
{AA 	
varBB 
	BlogModelBB 
=BB 
awaitBB !
	_BlogRepoBB" +
.BB+ ,

UpdateAyncBB, 6
(BB6 7
idBB7 9
,BB9 :
	updateDtoBB; D
)BBD E
;BBE F
ifCC 
(CC 
	BlogModelCC 
==CC 
nullCC !
)CC! "
{DD 
returnEE 
NotFoundEE 
(EE  
)EE  !
;EE! "
}FF 
returnHH 
OkHH 
(HH 
	BlogModelHH 
.HH  
	ToBlogDtoHH  )
(HH) *
)HH* +
)HH+ ,
;HH, -
}II 	
[JJ 	

HttpDeleteJJ	 
]JJ 
[KK 	
RouteKK	 
(KK 
$strKK 
)KK 
]KK 
publicMM 
asyncMM 
TaskMM 
<MM 
IActionResultMM '
>MM' (
DeleteMM) /
(MM/ 0
[MM0 1
	FromRouteMM1 :
]MM: ;
intMM< ?
idMM@ B
)MMB C
{NN 	
varOO 

stockModelOO 
=OO 
awaitOO "
	_BlogRepoOO# ,
.OO, -

DeleteAyncOO- 7
(OO7 8
idOO8 :
)OO: ;
;OO; <
ifQQ 
(QQ 

stockModelQQ 
==QQ 
nullQQ "
)QQ" #
{RR 
returnSS 
NotFoundSS 
(SS  
)SS  !
;SS! "
}TT 
returnUU 
OkUU 
(UU 

stockModelUU  
.UU  !
IDUU! #
+UU$ %
$strUU& .
)UU. /
;UU/ 0
}VV 	
}WW 
}XX �G
\D:\chuyên ngành\kì 5\Project P+N\Falgo_Project\Falgo\api\Controllers\AccountController.cs
	namespace 	
api
 
. 
Controllers 
{ 
[ 
Route 

(
 
$str 
) 
] 
[ 
ApiController 
] 
public 

class 
AccountController "
:# $
ControllerBase% 3
{ 
private 
readonly 
UserManager $
<$ %
AppUser% ,
>, -
_userManager. :
;: ;
private 
readonly 
SignInManager &
<& '
AppUser' .
>. /
_signInManager0 >
;> ?
private 
readonly 
IConfiguration '
_configuration( 6
;6 7
private 
readonly 
ITokenService &
_tokenService' 4
;4 5
public 
AccountController  
(  !
UserManager! ,
<, -
AppUser- 4
>4 5
userManager6 A
,A B
ITokenServiceC P
tokenServiceQ ]
,] ^
SignInManager_ l
<l m
AppUserm t
>t u
signInManager	v �
,
� �
IConfiguration
� �
configuration
� �
)
� �
{ 	
_userManager 
= 
userManager &
;& '
_signInManager 
= 
signInManager *
;* +
_configuration 
= 
configuration *
;* +
_tokenService 
= 
tokenService (
;( )
} 	
["" 	
HttpPost""	 
("" 
$str"" 
)"" 
]"" 
public## 
async## 
Task## 
<## 
IActionResult## '
>##' (
Login##) .
(##. /
[##/ 0
FromBody##0 8
]##8 9
LoginDto##: B
loginDto##C K
)##K L
{$$ 	
if%% 
(%% 
!%% 

ModelState%% 
.%% 
IsValid%% #
)%%# $
{&& 
return'' 

BadRequest'' !
(''! "

ModelState''" ,
)'', -
;''- .
}(( 
var** 
user** 
=** 
await** 
_userManager** )
.**) *
FindByNameAsync*** 9
(**9 :
loginDto**: B
.**B C
Username**C K
)**K L
;**L M
if++ 
(++ 
user++ 
==++ 
null++ 
)++ 
return++ $
Unauthorized++% 1
(++1 2
$str++2 D
)++D E
;++E F
var-- 
result-- 
=-- 
await-- 
_signInManager-- -
.--- .$
CheckPasswordSignInAsync--. F
(--F G
user--G K
,--K L
loginDto--M U
.--U V
Password--V ^
,--^ _
false--` e
)--e f
;--f g
if.. 
(.. 
!.. 
result.. 
... 
	Succeeded.. !
)..! "
return..# )
Unauthorized..* 6
(..6 7
$str..7 a
)..a b
;..b c
return00 
Ok00 
(00 
new00 

NewUserDto00 $
{11 
Id22 
=22 
user22 
.22 
Id22 
,22 
UserName33 
=33 
user33 
.33  
UserName33  (
,33( )
Email44 
=44 
user44 
.44 
Email44 "
,44" #
Token55 
=55 
_tokenService55 %
.55% &
CreateToken55& 1
(551 2
user552 6
)556 7
,557 8
DateOfBirth66 
=66 
user66 "
.66" #
DateOfBirth66# .
}77 
)77 
;77 
}88 	
[:: 	
HttpPost::	 
(:: 
$str:: 
):: 
]:: 
public;; 
async;; 
Task;; 
<;; 
IActionResult;; '
>;;' (
Register;;) 1
(;;1 2
[;;2 3
FromBody;;3 ;
];;; <
RegisterDto;;= H
registerDto;;I T
);;T U
{<< 	
if== 
(== 
!== 

ModelState== 
.== 
IsValid== #
)==# $
{>> 
foreach?? 
(?? 
var?? 
error?? "
in??# %

ModelState??& 0
.??0 1
Values??1 7
.??7 8

SelectMany??8 B
(??B C
v??C D
=>??E G
v??H I
.??I J
Errors??J P
)??P Q
)??Q R
{@@ 
ConsoleAA 
.AA 
	WriteLineAA %
(AA% &
errorAA& +
.AA+ ,
ErrorMessageAA, 8
)AA8 9
;AA9 :
}BB 
returnCC 

BadRequestCC !
(CC! "

ModelStateCC" ,
)CC, -
;CC- .
}DD 
varGG 
isUserExistsGG 
=GG 
awaitGG $
_userManagerGG% 1
.GG1 2
UsersGG2 7
.GG7 8
AnyAsyncGG8 @
(GG@ A
uGGA B
=>GGC E
uGGF G
.GGG H
UserNameGGH P
==GGQ S
registerDtoGGT _
.GG_ `
UsernameGG` h
)GGh i
;GGi j
varHH 
isEmailExistsHH 
=HH 
awaitHH  %
_userManagerHH& 2
.HH2 3
UsersHH3 8
.HH8 9
AnyAsyncHH9 A
(HHA B
uHHB C
=>HHD F
uHHG H
.HHH I
EmailHHI N
==HHO Q
registerDtoHHR ]
.HH] ^
EmailHH^ c
)HHc d
;HHd e
ifII 
(II 
isUserExistsII 
||II 
isEmailExistsII  -
)II- .
returnJJ 

BadRequestJJ !
(JJ! "
$strJJ" E
)JJE F
;JJF G
varLL 
userLL 
=LL 
newLL 
AppUserLL "
{MM 
UserNameNN 
=NN 
registerDtoNN &
.NN& '
UsernameNN' /
,NN/ 0
EmailOO 
=OO 
registerDtoOO #
.OO# $
EmailOO$ )
,OO) *
FullNamePP 
=PP 
registerDtoPP &
.PP& '
FullNamePP' /
,PP/ 0
	CreatedAtQQ 
=QQ 
DateTimeQQ $
.QQ$ %
UtcNowQQ% +
,QQ+ ,
DateOfBirthRR 
=RR 
registerDtoRR )
.RR) *
DateOfBirthRR* 5
,RR5 6
PhoneNumberSS 
=SS 
registerDtoSS )
.SS) *
PhoneNumberSS* 5
,SS5 6
AddressTT 
=TT 
registerDtoTT %
.TT% &
AddressTT& -
}UU 
;UU 
varWW 

createUserWW 
=WW 
awaitWW "
_userManagerWW# /
.WW/ 0
CreateAsyncWW0 ;
(WW; <
userWW< @
,WW@ A
registerDtoWWB M
.WWM N
PasswordWWN V
)WWV W
;WWW X
ifXX 
(XX 
!XX 

createUserXX 
.XX 
	SucceededXX %
)XX% &
returnXX' -

BadRequestXX. 8
(XX8 9

createUserXX9 C
.XXC D
ErrorsXXD J
)XXJ K
;XXK L
var[[ 

roleResult[[ 
=[[ 
await[[ "
_userManager[[# /
.[[/ 0
AddToRoleAsync[[0 >
([[> ?
user[[? C
,[[C D
$str[[E K
)[[K L
;[[L M
if\\ 
(\\ 
!\\ 

roleResult\\ 
.\\ 
	Succeeded\\ %
)\\% &
return\\' -

StatusCode\\. 8
(\\8 9
$num\\9 <
,\\< =

roleResult\\> H
.\\H I
Errors\\I O
)\\O P
;\\P Q
return^^ 
Ok^^ 
(^^ 
new^^ 

NewUserDto^^ $
{__ 
Id`` 
=`` 
user`` 
.`` 
Id`` 
,`` 
UserNameaa 
=aa 
useraa 
.aa  
UserNameaa  (
,aa( )
Emailbb 
=bb 
userbb 
.bb 
Emailbb "
,bb" #
Tokencc 
=cc 
_tokenServicecc %
.cc% &
CreateTokencc& 1
(cc1 2
usercc2 6
)cc6 7
,cc7 8
DateOfBirthdd 
=dd 
userdd "
.dd" #
DateOfBirthdd# .
}ee 
)ee 
;ee 
}gg 	
}ii 
}jj 
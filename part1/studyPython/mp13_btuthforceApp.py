# 암호해제 앱 
import itertools
import time
import zipfile

passwd_srting = '0123456789'    # 패스워드에 영문자도 들어있으면
# passwd_srting = '0123456789abcdefghijklmnopqrstuvwxyz...XYZ'

file = zipfile.ZipFile('./studyPython/암호는.zip')
isFind = False     # 암호를 찾았는지 

for i in range(1, 5):
    attempts = itertools.product(passwd_srting, repeat=i)
    for attempt in attempts:
        try_pass = ''.join(attempt)
        print(try_pass)

        time.sleep(0.05)
        try:
            file.extractall(pwd=try_pass.encode(encoding='utf-8'))
            print(f'암호는 {try_pass} 입니다.')
            isFind = True; break
        except:
            pass

    if isFind == True : break   # for 문을 빠져나가기 위해서 

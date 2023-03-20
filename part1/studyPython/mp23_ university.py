# 전국 대학교 지도 표시 
# pandas : 데이터 분석 최적화 된 모듈 
# pip install pandas

import numpy as np
import pandas as pd

filePath = './studyPython/university_list_2020.xlsx'
df_excel = pd.read_excel(filePath, engine='openpyxl')
df_excel.columns= df_excel.loc[4].tolist()
df_excel = df_excel.drop(index=list(range(0,5)))

print(df_excel.head())    # 상위 5개만 출력 

print(df_excel['학교명'].values)
print(df_excel['주소'].values)



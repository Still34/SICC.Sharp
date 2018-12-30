SUM   START   4000
FIRST LDX     ZERO
      LDA     ZERO
LOOP  STA     TABLE,X
      ADD     ONE
      ADD     TWO
      TIX     MAX
      JLT     LOOP
      RSUB
TABLE RESB    1024
MAX   WORD    100
ZERO  WORD    0
ONE   WORD    1
TWO   WORD    2
      END     FIRST
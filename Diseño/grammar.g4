@header{
import  java.util.HashMap; 
}
@members{
HashMap memory =  new  HashMap(); 
}

BRACKET_OPEN: '{' ;
BRACKET_CLOSE: '}' ;
NOMBRE: [a-zA-Z][a-zA-Z0-9]*;
NUMERO: DIGIT+ ;
DIGIT: ('0' .. '9') ;
SUM: '+' ;
RES: '-' ;
DIV: '/' ;
MUL: '*' ;
WS: [ \r\n\t] + -> channel (HIDDEN) ;

expresion: termino operacion termino ;
termino: expresion | const | NUMERO;
const: BRACKET_OPEN NOMBRE BRACKET_CLOSE;
operacion: SUM | RES | DIV | MUL ;
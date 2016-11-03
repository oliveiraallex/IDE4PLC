﻿'From Pharo4.0 of 18 March 2013 [Latest update: #40626] on 16 October 2016 at 10:37:07.42075 am'!Smalltalk renameClassNamed: #PLC_ErrorOpenad as: #PLC_ErrorOperand!Object subclass: #PLC_ErrorOperand	instanceVariableNames: 'dataValue errorText '	classVariableNames: ''	poolDictionaries: ''	category: 'IDE4PLC-OperandsAndAssignments'!PLC_SignedIntegers subclass: #PLC_UnspecifiedInteger	instanceVariableNames: ''	classVariableNames: ''	poolDictionaries: ''	category: 'IDE4PLC-DataTypes'!!PLC_UnspecifiedInteger commentStamp: 'CarlosLombardi 10/14/2016 21:09' prior: 0!Copyright © 2012-2015 Eric Nicolás Pernia.This class is part of IDE4PLC.IDE4PLC is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.IDE4PLC is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.You should have received a copy of the GNU Lesser General Public License along with IDE4PLC. If not, see <http://www.gnu.org/licenses/>.------------------------------------------------------------------A PLC_UnspecifiedInteger is xxxxxxxxx.Instance Variables!!PLC_ActualArgument methodsFor: 'checking' stamp: 'CarlosLombardi 10/16/2016 10:27'!typeError: operand ifAssignedTo: varDecl	"I do the typechecking corresponding to the assignment	   varDecl := operand	 and return the resulting error message, a String.	 If the operand passes the typecheck, then I return nil "		| errorText |	errorText := nil .  "I make this explicit"		(errorText isNil & (varDecl dataType acceptType: operand dataType) not)		ifTrue: [ errorText := 'Tipos incompatibles' ].			^errorText! !!PLC_ActualArgument methodsFor: 'checking' stamp: 'CarlosLombardi 10/16/2016 10:22'!check: rawOperand in: aPLC_ConnectableBlock	"I check the given raw operand. The result of the check is an operand as well. 	 It could be either:	 - the same raw operand, if I can accept it as is.	 - a specified operand, if the raw operand is not fully specified and I can find a way	   to make it complete.	 - an error operand, if I reject the raw operand						   ______ 	 			   |		  | 				   |		  |		  raw  --|	     |--  			 	   |______|		"		| varDecl errorText operand |	"If the rawOperand is already an error, just return it"		rawOperand isError ifTrue: [ ^rawOperand ].	operand := rawOperand.	errorText := nil.			"Obtengo la declaración del parametro correspondiente a este argumento"	varDecl := self parameterDeclIn: aPLC_ConnectableBlock .	"posible transformacion"	(operand canBeCoercedTo: varDecl dataType) ifTrue: [ 		operand := operand coercedTo: varDecl dataType 	] .	errorText := self typeError: operand ifAssignedTo: varDecl .	"veo si hubo algun error"	errorText ifNotNil: [ :text | 		operand := PLC_ErrorOperand new 			dataValue: operand asSimpleString ; 			errorText: errorText ; 			yourself ] .			"fin"	^ operand! !!PLC_EditorPOUDeclarations methodsFor: 'declarations Commands' stamp: 'CarlosLombardi 10/2/2016 20:15'!removeDeclaration	"Este comando modifica el modelo eliminando la declaracion de variable seleccionada"	| decl identfier row |	 ( self keyboardFocusMorph selectedIndex > 0 )		ifTrue: [ 					row := self keyboardFocusMorph morphsAt:  				self keyboardFocusMorph selectedIndex.							row 				ifNil: [ 					self keyboardFocusMorph selectedIndex: 0.				] 				ifNotNil: [ 					identfier := ( row at: 2) text.				] .							decl := pouDeclarations varDeclWithIdentifierOrNIl: identfier.			decl				ifNil: [					GrowlMorph 						openWithLabel: 'Editor POU Var Decls -> GrowlMorph -> Remove err1' translated						contents: 'Editor POU Var Decls -> GrowlMorph -> Remove err1 help' translated.							]				ifNotNil: [					"Remueve una declaracion de variable."					( (pouDeclarations varDecls remove: decl) isNil 						and: [ (pouDeclarations tempVarDecls remove: decl) isNil ] 					) 						ifTrue: [							GrowlMorph 								openWithLabel: 'Editor POU Var Decls -> GrowlMorph -> Remove err2' translated								contents: decl identifier.									]						ifFalse: [							"Refresco la grilla"							self refreshKeyboardFocusMorph.							GrowlMorph 								openWithLabel: 'Editor POU Var Decls -> GrowlMorph -> Remove err3' translated								contents: decl identifier.									].					].		]		ifFalse: [ 					self keyboardFocusMorph selectedIndex: 0.		].	! !!PLC_ErrorOperand methodsFor: 'displaying' stamp: 'CarlosLombardi 10/14/2016 15:30'!asSimpleString	"a String representing myself"	^self dataValue asString! !!PLC_ErrorOperand methodsFor: 'coercion' stamp: 'CarlosLombardi 10/14/2016 19:10'!canBeCoercedTo: aDataType	"whether it is possible for this operand to be coerced to the given type.	 Only literals can be coerced"	^false	! !!PLC_ErrorOperand methodsFor: 'accessing' stamp: 'CarlosLombardi 10/14/2016 12:23'!errorText	^ errorText! !!PLC_ErrorOperand methodsFor: 'accessing' stamp: 'CarlosLombardi 10/14/2016 12:23'!errorText: anObject	errorText := anObject! !!PLC_Variable methodsFor: 'coercion' stamp: 'CarlosLombardi 10/14/2016 19:10'!canBeCoercedTo: aDataType	"whether it is possible for this operand to be coerced to the given type.	 Only literals can be coerced"	^false! !!PLC_Variable methodsFor: 'displaying' stamp: 'CarlosLombardi 10/14/2016 15:27'!asSimpleString	"a String representing myself"	^self declaration identifier asString! !!IDE4PLCParsers class methodsFor: 'as yet unclassified' stamp: 'CarlosLombardi 10/14/2016 20:51'!literalINTParser	"a parser for a INT literal, understood as signed integer.		(INT#)?integer "	| intType intLiteral |	intType := 'INT#' asParser.	intLiteral := (intType , BasicParsers integerNumber) end ==> [ :nodes | 		PLC_Literal			newWithDataType: PLC_DataType Int				andDataValue: nodes second 	] .	^intLiteral	! !!IDE4PLCParsers class methodsFor: 'as yet unclassified' stamp: 'CarlosLombardi 10/15/2016 11:51'!unspecifiedIntegerParser	"a parser for an unsigned integer literal, with no specified type.		(INT#)?integer "	| intLiteral |	intLiteral := BasicParsers integerNumber end ==> [ :num | 		PLC_Literal			newWithDataType: PLC_DataType UnspecifiedInt 			andDataValue: num	] .	^intLiteral	! !!PLC_ConnectableBlocksNetwork methodsFor: 'commands add-remove Elements' stamp: 'CarlosLombardi 10/14/2016 12:59'!changeActualArgumentFor: aPLC_ConnectableBlock by: aString	"Cambia el Actual Argument. "	"HACER: 		1 - le pase el string a su pou para que lo convierta en un objeto operando ( PARSEO )		| operand |		Devuelve un objeto que puede ser operando: variable, item de estructura, literal, o void;	o devolver nil si el string es cualquier otra cosa.		operand := self pouBody pou convertStringToOperand: aString		2a - Si es operando, le pido al elemento actual argument del bc que le llega como parametro	que cheque si le cabe que le asignen el operando. 	Este operando seguro entiende dataType y dataValue. 	Si es variable o item de estructura además se puede buscar su declaración 		Si le cabe se pone el flag de 'valorInvalido' a False en el actual argument 		Si no le cabe se pone el flag de 'valorInvalido' a True en el actual argument 			Siempre se lo asigna (cambia el texto auque se ingrese cualquier verdura), 			con el flag se determina si se dibuja en rojo o negro para indicar que se 			va a poder compilar o no.		2b - Si es nil le guarda un operando void al elemento actual argument.						LO DE ABAJO ES UN PARCHE PARA QUE SIGA ANDANDO, DESPUES HAY QUE SACAR TODO....	"			| argument |		"UPDATE 22/09/2016 - nuevo parser más bonito, que usa PetitParser."	argument := self parse: aString.						"new, october 2016 - operand check that can result in a different operand, or an error operand"	argument := aPLC_ConnectableBlock element check: argument in: aPLC_ConnectableBlock.	argument isError ifTrue: [ 		"Aviso en pantalla que no encontre la variable. 		 ESTA MUY MAL ACA ESTO, DEBERIA HACERLO LA VISTA"		GrowlMorph openWithLabel: 'Error' contents: argument errorText .			] .		"Cambia el argumento actual"	aPLC_ConnectableBlock element actualArgument: argument.		^ true.! !!PLC_ConnectableBlocksNetwork methodsFor: 'PARSER' stamp: 'CarlosLombardi 10/2/2016 20:09'!oldParse: aString	"PARSEO LO QUE LLEGUE ASI BUSCO LA VARIABLE CORRECTA"	"aString Parseado pedorramente - Es true, false o numero 	(entero o time)"		| pouDecls decl  argument |		pouDecls := self pouBody pou declarations.			decl := pouDecls varDeclWithIdentifierOrNIl: aString.		decl 		ifNil: [			"No encontro declaracion. Chequeo si puedo convertir aString 			a un literal, sino creo una nueva variable de la galera con ese			identifier."						"Parseo de Literales barato"							"Pruebo si el string es TRUE"				( aString asLowercase = 'true' )				ifTrue: [					"Si era true creo un Literal true"					argument := ( PLC_Literal 						newWithDataType: PLC_DataType Bool						andDataValue: true	).				]				ifFalse: [					"Pruebo si el string es FALSE"					( aString asLowercase = 'false' )						ifTrue: [ 							"Si era false creo un Literal false"							argument := ( PLC_Literal 								newWithDataType: PLC_DataType Bool								andDataValue: false ).						]						ifFalse: [ 							"Pruebo si es numerico ENTERO"							aString isAllDigits								ifTrue: [ 									"Si era numero creo un Literal INT"									argument := ( PLC_Literal 										newWithDataType: PLC_DataType Int										andDataValue: aString asNumber ).								]								ifFalse: [									"Chequea si empieza con T# o TIME# para parsear el numero como TIME"									( ( aString asUppercase beginsWith: 'T#' ) or: 									[ aString asUppercase beginsWith: 'TIME#'  ] )										ifTrue: [ 											"Si era numero creo un Literal TIME"											argument := ( PLC_Literal 												newWithDataType: PLC_DataType Time												andDataValue: aString extractNumber ).										] 										ifFalse: [ 											"NO PUDE PARSEARLO" 																						"INVENTO UNA DECLARACION DE VARIABLE"											"varDecl := PLC_SymbolicVariableDecl 												newWithDataType: PLC_DataType Void												andInitialValue: nil 												andIdentifier: aString."																			"Creo el Operando Variable a partir de la Declaración de Variable"											"argument := PLC_Variable 												newWithDeclaration: varDecl 												andDataValue: nil."											"Creo un VoidOperand"												argument := PLC_ErrorOperand new.											argument dataValue: aString.																						"Aviso en pantalla que no encontre la variable. 											ESTA MUY MAL ACA ESTO, DEBERIA HACERLO 											LA VISTA"											GrowlMorph 												openWithLabel: 'Error' 												contents: 'Variable no encontrada entre las declaraciones o valor inválido'.																					].								]. 						]. 				].		]		ifNotNil: [			"Encontro declaracion, creo un nuevo Operando variable 			con la declaración hallada"			argument := PLC_Variable 				newWithDeclaration: decl 				andDataValue: decl initialValue.		].			^ argument.! !!PLC_ConnectableBlocksNetwork methodsFor: 'PARSER' stamp: 'CarlosLombardi 10/14/2016 20:56'!parse: aString	"parse an operand into a PLC_Literal or PLC_Variable.	 If aString is an invalid operand, then a PLC_ErrorOpenad is returned. 	 In this case, an error message is also shown (shame!! for not delegating the message display to GUI components). "		| pouDecls variableParser operandParser result errorText |		pouDecls := self pouBody pou declarations.		variableParser := IDE4PLCParsers idParser ==> [ :idString |		| decl |		decl := pouDecls varDeclWithIdentifierOrNIl: aString.		decl ifNil: [				PLC_ErrorOperand new dataValue: idString ; yourself			] ifNotNil: [ 				"A declaration is found, I create a new Variable operand for the found declaration"				PLC_Variable 					newWithDeclaration: decl 					andDataValue: decl initialValue.			]	] .		operandParser := 		IDE4PLCParsers literalBOOLParser /			IDE4PLCParsers unspecifiedIntegerParser /		IDE4PLCParsers literalREALParser /		IDE4PLCParsers literalINTParser /		IDE4PLCParsers literalTIMEParser /		variableParser .		result := operandParser parse: aString.		errorText := nil .	result isPetitFailure ifTrue: [ 		"lexing error, I give different message but return similar error operand"			result := PLC_ErrorOperand new 			dataValue: aString ; 			errorText: 'Valor inválido' ;			yourself .	] ifFalse: [ 		"lexing OK, unknown identifier"			result isError ifTrue: [ 			result errorText: 'Variable no encontrada entre las declaraciones'.		] .	] .		^result! !!PLC_VariableDecl methodsFor: 'accept' stamp: 'CarlosLombardi 10/14/2016 15:00'!acceptVariable: aVariableOperand	"Responde true si su tipo de datos acepta el tipo.			El chequeo de si aVariableOaVariableOperand está entre las declaraciones 		de la POU que contiene a esta declaracion se hace luego del parseo."		^ self dataType acceptType: aVariableOperand dataType.		! !!PLC_VariableDecl methodsFor: 'accept' stamp: 'CarlosLombardi 10/14/2016 14:59'!acceptLiteral: aLiteral	"Responde true si:		Su tipo de datos acepta el valor y		Su tipo de datos acepta el tipo y		Su categoría  acepta literal"		^ ( self dataType acceptValue: aLiteral dataValue ) 		and: [ 			( self dataType acceptType: aLiteral dataType )				and: [					self variableCategory acceptLiteral					]			].! !!PLC_ActualArgumentReader methodsFor: 'accessing' stamp: 'CarlosLombardi 10/2/2016 23:59'!parameterDeclIn: aPLC_ConnectableBlock 	"The declaration of the parameter corresponding to this argument"		| inputPinNumber otherConnectableBlock varDecl |		"Obtengo el número de pin de entrada al que está conectado el pin de salida único este Actual Argument"	inputPinNumber := aPLC_ConnectableBlock inputPinNumberConnectedToOutput: 1.		"Obtengo el bloque conectable al que está conectado este Actual Argument"	otherConnectableBlock := aPLC_ConnectableBlock blockConnectedToOutput: 1.		"Obtengo la declaración de variable de la entrada numero inputPinNumber de otherConnectableBlock"	varDecl := otherConnectableBlock element inputs at: inputPinNumber.		^varDecl ! !!PLC_ActualArgumentReader methodsFor: 'accept' stamp: 'CarlosLombardi 10/2/2016 23:35'!acceptLiteral: aLiteralOperand in: aPLC_ConnectableBlock	"Devuelve true acepta el operando que le llega como parámetro"	"				   ______ 	 			   |		  | 				   |		  |		   10  --|	     |--  			 	   |______|		"		| varDecl |		"Obtengo la declaración del parametro correspondiente a este argumento"	varDecl := self parameterDeclIn: aPLC_ConnectableBlock .		"CHEQUEO - Le pregunto a la variable si acepta el valor y el tipo del literal.	Le pregunta tambien a su categoria: >>acceptLiteral"	^ ( varDecl acceptLiteral: aLiteralOperand )! !!PLC_ActualArgumentReader methodsFor: 'accept' stamp: 'CarlosLombardi 10/2/2016 23:36'!acceptVariable: aVariableOperand in: aPLC_ConnectableBlock	"Devuelve true acepta el operando que le llega como parámetro				   ______ 	 			   |		  | 				   |		  |	     VAR  --|	    |--  			 	   |______|		"		"Hacer chequeos que faltan"	| varDecl |		"1 - PRIEMER CHEQUEO - Chequea si la varaible que quiero asignar puede leerse o no.	Si no puede, sale, si puede sigue chequeando."	( self canReadVariable: aVariableOperand )		ifFalse: [ ^ false ].		"Obtengo el parametro correspondiente a este argumento"	varDecl := self parameterDeclIn: aPLC_ConnectableBlock .			"2 - SEGUNDO CHEQUEO - Le pregunto a la variable si acepta el tipo de aVariableOperand.	Si da falso ya sale y sino hace el segundo chequeo"	^ varDecl acceptVariable: aVariableOperand.! !!PLC_ActualArgumentReader methodsFor: 'checking' stamp: 'CarlosLombardi 10/16/2016 10:28'!typeError: operand ifAssignedTo: varDecl	"I do the typechecking corresponding to the assignment	   varDecl := operand	 and return the resulting error message, a String.	 If the operand passes the typecheck, then I return nil "		| errorText |		errorText := super typeError: operand ifAssignedTo: varDecl .	(errorText isNil & operand dataType isPLCUnspecifiedType)		ifTrue: [ errorText := 'El valor no corresponde al tipo del argumento' ].	(errorText isNil & operand isPLCLiteral) ifTrue: [ 		(varDecl dataType acceptValue: operand dataValue)			ifFalse: [ errorText := 'Valor fuera de rango' ]	] .	^errorText! !!PasteUpMorph methodsFor: 'event handling' stamp: 'CarlosLombardi 10/15/2016 12:06'!handlesMouseDown: evt		^true.! !!PLC_POU_Declarations methodsFor: 'find' stamp: 'CarlosLombardi 10/14/2016 12:49'!varDeclWithIdentifierOrNIl: aVariableIdentifier	"La delaracion de variable con el identificador aVariableIdentifier.	 Si no hay, devuelve nil"	self userVarDecls do: [ :each |		( each varDeclWithIdentifierOrNIl: aVariableIdentifier )			ifNotNil: [ ^ each varDeclWithIdentifierOrNIl: aVariableIdentifier ].		 ].		^ nil.! !!PLC_Resource_Declarations methodsFor: 'find' stamp: 'CarlosLombardi 10/2/2016 20:14'!varDeclWithIdentifierOrNIl: aVariableIdentifier	"La delaracion de variable con el identificador aVariableIdentifier.	 Si no hay, devuelve nil"	^ self userVarDecls varDeclWithIdentifierOrNIl: aVariableIdentifier.	! !!PLC_Literal methodsFor: 'displaying' stamp: 'CarlosLombardi 10/14/2016 15:25'!asSimpleString	"a String representing myself"	^self dataValue	 asString! !!PLC_Literal methodsFor: 'coercion' stamp: 'CarlosLombardi 10/14/2016 21:28'!canBeCoercedTo: aDataType	"whether it is possible for this operand to be coerced to the given type.	 Only literals can be coerced"	^self dataType canCoerce: self dataValue to: aDataType	! !!PLC_Literal methodsFor: 'coercion' stamp: 'CarlosLombardi 10/15/2016 11:02'!coercedTo: aDataType	"the result of my coercion to the given type.	 An error is raised if the coercion is not possible.	 Recall: only literals can be coerced"	(self canBeCoercedTo: aDataType) ifFalse: [ 		self error: ('This operand cannot be coerced to ', aDataType description)	] .	^PLC_Literal new 		dataValue: self dataValue ;		dataType: (self dataType typeToCoerce: self dataValue to: aDataType) ;		yourself	! !!PLC_DataType methodsFor: 'testing' stamp: 'CarlosLombardi 10/15/2016 10:51'!isPLCIntegerType	"whether I represent an integer type"	^false! !!PLC_DataType methodsFor: 'testing' stamp: 'CarlosLombardi 10/15/2016 10:52'!isPLCSignedType	"whether I represent an numerical signed type"	^false! !!PLC_DataType methodsFor: 'testing' stamp: 'CarlosLombardi 10/15/2016 11:07'!isPLCUnspecifiedType	"whether I represent the temporary type assigned to a literal whose precise type	 is to be unspecified "	^false! !!PLC_DataType methodsFor: 'coercion' stamp: 'CarlosLombardi 10/14/2016 21:29'!canCoerce: aValue to: aDataType	"whether it is possible for a literal of this type having the specified value, 	 to be coerced to the given type"	^false	! !!PLC_SignedIntegers methodsFor: 'testing' stamp: 'CarlosLombardi 10/15/2016 10:51'!isPLCIntegerType	"whether I represent an integer type"	^true! !!PLC_SignedIntegers methodsFor: 'testing' stamp: 'CarlosLombardi 10/15/2016 10:52'!isPLCSignedType	"whether I represent an numerical signed type"	^true! !!PLC_UnspecifiedInteger methodsFor: 'types' stamp: 'CarlosLombardi 10/14/2016 21:10'!description	"Devuelve la descripción del tipo de dato."	^ 'Entero sin especificar.'.! !!PLC_UnspecifiedInteger methodsFor: 'types' stamp: 'CarlosLombardi 10/14/2016 21:10'!bitSize	"Devuelve el tamaño del tipo de dato en cantidad de bits."	^ 32.! !!PLC_UnspecifiedInteger methodsFor: 'types' stamp: 'CarlosLombardi 10/14/2016 21:13'!keyword	"Devuelve el keyword del tipo de dato."	^ #UNSP_INT.! !!PLC_UnspecifiedInteger methodsFor: 'coercion' stamp: 'CarlosLombardi 10/15/2016 10:56'!canCoerce: aValue to: aDataType	"whether it is possible for a literal of this type having the specified value, 	 to be coerced to the given type"	^aDataType isPLCIntegerType & ((aValue >= 0) | aDataType isPLCSignedType not)	! !!PLC_UnspecifiedInteger methodsFor: 'coercion' stamp: 'CarlosLombardi 10/15/2016 11:05'!typeToCoerce: aValue to: aDataType	"the data type to coerce the specified value to the given type"	^aDataType 	! !!PLC_UnspecifiedInteger methodsFor: 'testing' stamp: 'CarlosLombardi 10/15/2016 11:06'!isPLCUnspecifiedType	"whether I represent the temporary type assigned to a literal whose precise type	 is to be unspecified "	^true! !!PLC_UnsignedIntegers methodsFor: 'testing' stamp: 'CarlosLombardi 10/15/2016 10:53'!isPLCIntegerType	"whether I represent an integer type"	^true! !!PLC_ActualArgumentWriter methodsFor: 'accessing' stamp: 'CarlosLombardi 10/14/2016 18:12'!parameterDeclIn: aPLC_ConnectableBlock 	"The declaration of the parameter corresponding to this argument"		| outputPinNumber otherConnectableBlock varDecl |		"Obtengo el número de pin de entrada al que está conectado el pin de salida único este Actual Argument"	outputPinNumber := aPLC_ConnectableBlock outputPinNumberConnectedToInput: 1.		"Obtengo el bloque conectable al que está conectado este Actual Argument"	otherConnectableBlock := aPLC_ConnectableBlock blockConnectedToInput: 1.		"Obtengo la declaración de variable de la entrada numero inputPinNumber de otherConnectableBlock"	varDecl := otherConnectableBlock element outputs at: outputPinNumber.		^varDecl ! !!PLC_ActualArgumentWriter methodsFor: 'check' stamp: 'CarlosLombardi 10/16/2016 10:30'!typeError: operand ifAssignedTo: varDecl	"I do the typechecking corresponding to the assignment	   varDecl := operand	 and return the resulting error message, a String.	 If the operand passes the typecheck, then I return nil "		| errorText |		errorText := super typeError: operand ifAssignedTo: varDecl .	(errorText isNil & operand isPLCLiteral) ifTrue: [ 		errorText := 'Un valor de salida no puede ser literal' 	] .	^errorText! !!PLC_VariableCategoryDecl methodsFor: 'find' stamp: 'CarlosLombardi 10/2/2016 20:14'!varDeclWithIdentifierOrNIl: aVariableIdentifier	"La delaracion de variable con el identificador aVariableIdentifier.	 Si no hay, devuelve nil"	^ self declarations 		detect: [ :each |			each identifier = aVariableIdentifier			 ] 		ifNone: [			^ nil.				]. ! !!PLC_VariableDeclController methodsFor: 'commands recived from dialog' stamp: 'CarlosLombardi 10/2/2016 20:16'!ok	"llega cuando apretan Aceptar en el dialog""	Transcript open.		Transcript show: category selectedItem.	Transcript show: ''.	Transcript show: identifier contents.	Transcript show: ''.	Transcript show: dataType selectedItem."		"Chequeo que tenga un modelo"	( self varDecls ) 		ifNotNil: [									(self varDecls varDeclWithIdentifierOrNIl: self getIdentifier)				ifNil: [					"Agrego una declaracion de variable 					Cuando hace add le agrega su categoría automaticamente"					( self varDecls perform: self getCategorySelector )						add: ( 							PLC_SymbolicVariableDecl 								newWithDataType: self getDataType								andInitialValue: nil 								andIdentifier: self getIdentifier						).						"Refresco la grilla"					self editor refreshKeyboardFocusMorph.				]				ifNotNil: [					GrowlMorph 						openWithLabel: 'Editor POU Var Decls -> GrowlMorph -> Identifier error 2 -> title' translated						contents: 'Editor POU Var Decls -> GrowlMorph -> Identifier error 2' translated.				].								].		self close.! !!PLC_DataType class methodsFor: 'ElementaryDataTypes' stamp: 'CarlosLombardi 10/15/2016 11:35'!UnspecifiedInt	"Retorna la unica instancia del tipo Unspecified Int"	TypesCreated ifFalse: [self createDataTypes].		^ ElementaryDataTypes at: #UNSP_INT.		"Bool Byte Word DWord LWord SInt Int DInt LInt USInt UInt UDInt ULInt Real LReal String WString Time Date TimeOfDay TOD DateAndTime DT"! !!PLC_DataType class methodsFor: 'instance creation' stamp: 'CarlosLombardi 10/15/2016 11:35'!createElementaryDataTypes	"Crea los tipos de datos elementales"	| typeTemp |		"Bit strings"		typeTemp := PLC_Void new.	ElementaryDataTypes at: typeTemp keyword put: typeTemp.		typeTemp := PLC_Boolean new.	ElementaryDataTypes at: typeTemp keyword put: typeTemp.		typeTemp := PLC_Byte new.	ElementaryDataTypes at: typeTemp keyword put: typeTemp.		typeTemp := PLC_Word new.	ElementaryDataTypes at: typeTemp keyword put: typeTemp.		typeTemp := PLC_DoubleWord new.	ElementaryDataTypes at: typeTemp keyword put: typeTemp.		typeTemp := PLC_LongWord new.	ElementaryDataTypes at: typeTemp keyword put: typeTemp.	"Unspecified Integer"	typeTemp := PLC_UnspecifiedInteger new.	ElementaryDataTypes at: typeTemp keyword put: typeTemp.		"Signed Integers"		typeTemp := PLC_ShortInteger new.	ElementaryDataTypes at: typeTemp keyword put: typeTemp.		typeTemp := PLC_Integer new.	ElementaryDataTypes at: typeTemp keyword put: typeTemp.		typeTemp := PLC_DoubleInteger new.	ElementaryDataTypes at: typeTemp keyword put: typeTemp.		typeTemp := PLC_LongInteger new.	ElementaryDataTypes at: typeTemp keyword put: typeTemp.		"Unsigned Integers"		typeTemp := PLC_UnsignedShortInteger new.	ElementaryDataTypes at: typeTemp keyword put: typeTemp.		typeTemp := PLC_UnsignedInteger new.	ElementaryDataTypes at: typeTemp keyword put: typeTemp.		typeTemp := PLC_UnsignedDoubleInteger new.	ElementaryDataTypes at: typeTemp keyword put: typeTemp.		typeTemp :=  PLC_UnsignedLongInteger new.	ElementaryDataTypes at: typeTemp keyword put: typeTemp.		"Reals"		typeTemp := PLC_Real new.	ElementaryDataTypes at: typeTemp keyword put: typeTemp.		typeTemp := PLC_LongReal new.	ElementaryDataTypes at: typeTemp keyword put: typeTemp.		"Character Strings"		typeTemp := PLC_SingleByteString new.	ElementaryDataTypes at: typeTemp keyword put: typeTemp.		typeTemp :=  PLC_DoubleByteString new.	ElementaryDataTypes at: typeTemp keyword put: typeTemp.		"Time and date"		typeTemp := PLC_Duration new.	ElementaryDataTypes at: typeTemp keyword put: typeTemp.		typeTemp :=  PLC_Date new.	ElementaryDataTypes at: typeTemp keyword put: typeTemp.		typeTemp := PLC_TimeOfDay new.	ElementaryDataTypes at: (typeTemp keywords at: 1) put: typeTemp.	ElementaryDataTypes at: (typeTemp keywords at: 2) put: typeTemp.		typeTemp :=  PLC_DateAndTimeOfDay new.	ElementaryDataTypes at: (typeTemp keywords at: 1) put: typeTemp.	ElementaryDataTypes at: (typeTemp keywords at: 2) put: typeTemp.! !PLC_VariableCategoryDecl removeSelector: #includeVarDeclWithIdentifier:!PLC_ActualArgumentWriter removeSelector: #check:in:!!PLC_ActualArgumentWriter reorganize!(#testing isActualArgumentWriter)(#accessing parameterDeclIn:)(#connectableBlocks connectionToAddElement:inBlock:)(#library keyword)(#accept acceptStructItem:in: accept:in: canWriteVariable: acceptVariable:in:)(#'initialize-release' initialize)(#check typeError:ifAssignedTo:)!!PLC_UnsignedIntegers reorganize!(#testing isPLCIntegerType)(#types minValue maxValue)!!PLC_UnspecifiedInteger reorganize!(#types description bitSize keyword)(#coercion canCoerce:to: typeToCoerce:to:)(#testing isPLCUnspecifiedType)!!PLC_SignedIntegers reorganize!(#types minValue maxValue)(#testing isPLCIntegerType isPLCSignedType)!PLC_DataType removeSelector: #canBeCoercedTo:!!PLC_DataType reorganize!(#types acceptType: dataType accept:)(#wired acceptWire)(#compilation ilCompileDataType cCompileDataType cCompile ilCompile)(#testing isPLCIntegerType isPLCSignedType isPLCUnspecifiedType)(#coercion canCoerce:to:)!PLC_Literal removeSelector: #isBitStringLiteral!PLC_Literal removeSelector: #isBooleanLiteral!PLC_Literal removeSelector: #isCharacterString!PLC_Literal removeSelector: #isNumericLiteral!!PLC_Literal reorganize!(#displaying asSimpleString)(#accessing dataValue dataType dataValue: dataType:)(#'actual argument' identifier)(#coercion canBeCoercedTo: coercedTo:)(#testingSubclass isTimeLiteral)(#compilation cCompile ilCompile)(#testing isPLCLiteral isError isVariable isStructItem isVoid)!PLC_Resource_Declarations removeSelector: #includeVarDeclWithIdentifier:!PLC_POU_Declarations removeSelector: #includeVarDeclWithIdentifier:!PLC_ActualArgumentReader removeSelector: #check:in:!!PLC_ActualArgumentReader reorganize!(#accessing parameterDeclIn:)(#accept acceptLiteral:in: acceptStructItem:in: canReadVariable: accept:in: acceptVariable:in:)(#library keyword)(#checking typeError:ifAssignedTo:)(#testing isActualArgumentReader)(#'initialize-release' initialize)!PLC_VariableDecl removeSelector: #acceptDataType:!!PLC_Variable reorganize!(#accessing declaration: dataType declaration dataValue identifier dataValue:)(#testing isVaraible isPLCLiteral isError isStructItem isVoid)(#coercion canBeCoercedTo:)(#'initialize-release' initialize)(#displaying asSimpleString)(#compilation varScopePrefix cCompile ilCompile)!Object subclass: #PLC_ErrorOperand	instanceVariableNames: 'dataValue errorText'	classVariableNames: ''	poolDictionaries: ''	category: 'IDE4PLC-OperandsAndAssignments'!!PLC_ErrorOperand reorganize!(#displaying asSimpleString)(#coercion canBeCoercedTo:)(#testing isVaraible isPLCLiteral isError isStructItem isVoid)(#compilation dataType ilCompile cCompile dataValue identifier dataValue:)(#accessing errorText errorText:)!!PLC_ActualArgument reorganize!(#accessing actualArgument actualArgument:)(#library keyword)(#checking typeError:ifAssignedTo: check:in:)(#'initialize-release' initialize)(#testing isActualArgumentWriter isActualArgumentReader)(#'graphic element' graphicElementFor:)(#connectableBlocks addOutput connectionToAddElement:inBlock: addInput)!
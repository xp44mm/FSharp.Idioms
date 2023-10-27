namespace FSharp.Idioms

open Xunit
open Xunit.Abstractions
open FSharp.Idioms.Literals
open FSharp.xUnit

type ListTest(output:ITestOutputHelper) =
    let show res = 
        res 
        |> Render.stringify
        |> output.WriteLine

    //static let splitData = [
    //    {|body=[];symbol="x";res=[]|}
    //    {|body=["y"];symbol="x";res=["y"]|}
    //    {|body=["y"];symbol="x";res=["y"]|}
    //]

    [<Fact>]
    member this.``Pair List toMap``() =
        // 有重复键
        let pairs = [
            1,1
            1,2
            1,2
            2,1
            2,1
            2,2
        ]

        let y = List.toMap pairs
        //show y
        let res = 
            Map.ofList [
                1,[1;2;2];
                2,[1;1;2];
                ]
        Should.equal y res

    [<Fact>]
    member this.``Triple List toJaggedMap``() =
        // 有重复键
        let triples = 
            [
                (1,1,1)
                (1,2,2)
                (1,2,3)
                (2,1,4)
                (2,1,5)
                (2,2,6)
            ]
        let y = List.toJaggedMap triples
        //show y
        let res = 
            Map.ofList [
                1,Map.ofList [
                    1,[1];
                    2,[2;3]
                    ];
                2,Map.ofList [
                    1,[4;5];
                    2,[6]
                    ];
                ]
        Should.equal y res

    [<Fact>]
    member this.``ofJaggedMap to Triple List``() =
        let jaggedMap =
            Map.ofList [
                1,Map.ofList [
                    1,1;
                    2,2;
                    ];
                2,Map.ofList [
                    1,3;
                    2,4;
                    ];
                ]

        let y = List.ofJaggedMap jaggedMap

        //show y
        let res = 
            [
                (1,1,1)
                (1,2,2)
                (2,1,3)
                (2,2,4)
            ]

        Should.equal y res


    [<Fact>]
    member this.``advanceWhile``() =
        let ls = [1;2;3;4;5]
        
        let y = 
            ls
            |> List.advanceWhile (fun i -> i<3)

        let res = [2;1],[3;4;5]
        Should.equal y res

    [<Fact>]
    member this.``advance``() =
        let ls = [1;2;3;4;5]
        
        let y = 
            ls
            |> List.advance 2

        let res = [2;1],[3;4;5]
        Should.equal y res

    [<Fact>]
    member this.``tap``() =
        let ls = [1;2;3;4;5]
        
        let y = 
            ls
            |> List.tap(fun x -> output.WriteLine(sprintf "%A" x))

        Should.equal ls y


    [<Fact>]
    member this.``ofRevArray``() =
        let arr = [|5;4;3;2;1|]
        
        let y = 
            arr
            |> List.ofRevArray

        let ls = [1;2;3;4;5]

        Should.equal y ls

    [<Fact>]
    member this.``depthFirstSort 22-4``() =
        let nodes = 
            [ "u",["v";"x"];
              "v",["y"];
              "w",["y";"z"];
              "x",["v"];
              "y",["x"];
              "z",["z"];
            ] |> Map.ofList
        
        let y = List.depthFirstSort nodes "u"
        show y
        //注意：结果列表忽略无关的元素
        let e = ["u";"v";"y";"x"]
        Should.equal e y
        ()

    [<Fact>]
    member this.``depthFirstSort 22-5``() =
        let nodes = 
            [ 
            "u",["t";"v"];
            "v",["s";"w"];
            "w",["x";];
            "x",["z"];
            "y",["x"];
            "z",["y";"w"];
            "s",["z";"w"];
            "t",["u";"v"];
            ] 
            |> Map.ofList
        
        let y = List.depthFirstSort nodes "s"
        show y
        //注意：结果列表忽略无关的元素
        let e = ["s";"z";"y";"x";"w"]
        Should.equal e y
        ()


    [<Fact>]
    member this.``depthFirstSort bnf``() =
        let nodes = Map [
            "abstractMemberFlags",["ABSTRACT";"MEMBER";"STATIC"];"access",["PRIVATE";"PUBLIC";"INTERNAL"];"activePatternCaseName",["IDENT"];"activePatternCaseNames",["BAR";"activePatternCaseName"];"anonRecdType",["STRUCT";"braceBarFieldDeclListCore"];"appExpr",["argExpr";"atomicExpr"];"appTypeCon",["path";"typar"];"appTypeConPower",["appTypeCon";"INFIX_AT_HAT_OP";"atomicRationalConstant"];"appTypePrefixArguments",["typeArgActual";"COMMA";"typeArgListElements"];"argExpr",["ADJACENT_PREFIX_OP";"atomicExpr"];"arrayExpr",["LBRACK_BAR";"arrayExprElements";"BAR_RBRACK";"recover";"error"];"arrayExprElements",["sequentialExpr"];"arrayTypeSuffix",["LBRACK";"RBRACK";"COMMA"];"arrowThenExprR",["RARROW";"typedSequentialExprBlockR"];"asSpec",["AS";"ident"];"atomType",["HASH";"appTypeConPower";"UNDERSCORE";"LPAREN";"typ";"rparen";"recover";"STRUCT";"appType";"STAR";"tupleOrQuotTypeElements";"rawConstant";"NULL";"CONST";"atomicExpr";"FALSE";"TRUE";"error";"appTypeCon";"typeArgsNoHpaDeprecated";"DOT";"path";"ends_coming_soon_or_recover"];"atomTypeNonAtomicDeprecated",["LPAREN";"appTypePrefixArguments";"rparen";"appTypeConPower";"atomType"];"atomTypeOrAnonRecdType",["atomType";"anonRecdType"];"atomicExpr",["HIGH_PRECEDENCE_BRACK_APP";"HIGH_PRECEDENCE_PAREN_APP";"HIGH_PRECEDENCE_TYAPP";"typeArgsActual";"PREFIX_OP";"QUOTE";"ident";"DOT";"atomicExprQualification";"BASE";"QMARK";"nameop";"dynamicArg";"GLOBAL";"identExpr";"LBRACK";"listExprElements";"RBRACK";"recover";"error";"STRUCT";"LPAREN";"tupleExpr";"rparen";"atomicExprAfterType"];"atomicExprAfterType",["constant";"parenExpr";"braceExpr";"braceBarExpr";"interpolatedString";"NULL";"FALSE";"TRUE";"quoteExpr";"arrayExpr";"beginEndExpr"];"atomicExprQualification",["identOrOp";"GLOBAL";"recover";"LPAREN";"COLON_COLON";"rparen";"DOT";"INT32";"typedSequentialExpr";"LBRACK";"RBRACK";"error"];"atomicPatsOrNamePatPairs",["LPAREN";"namePatPairs";"rparen";"atomicPatterns"];"atomicPattern",["quoteExpr";"CHAR";"DOT_DOT";"LBRACE";"recordPatternElementsAux";"rbrace";"LBRACK";"listPatternElements";"RBRACK";"LBRACK_BAR";"BAR_RBRACK";"UNDERSCORE";"QMARK";"ident";"atomicPatternLongIdent";"constant";"FALSE";"TRUE";"NULL";"LPAREN";"parenPatternBody";"rparen";"recover";"error";"STRUCT";"tupleParenPatternElements"];"atomicPatternLongIdent",["UNDERSCORE";"DOT";"pathOp";"GLOBAL";"access"];"atomicPatterns",["atomicPattern";"HIGH_PRECEDENCE_BRACK_APP";"HIGH_PRECEDENCE_PAREN_APP"];"atomicRationalConstant",["atomicUnsignedRationalConstant";"MINUS"];"atomicUnsignedRationalConstant",["INT32";"LPAREN";"rationalConstant";"rparen"];"attrUnionCaseDecl",["opt_attributes";"opt_access";"unionCaseName";"OF";"unionCaseRepr";"recover";"COLON";"topType";"EQUALS";"constant"];"attrUnionCaseDecls",["attrUnionCaseDecl";"barAndgrabXmlDoc"];"attr_localBinding",["opt_attributes";"localBinding";"error"];"attribute",["path";"opt_HIGH_PRECEDENCE_APP";"opt_atomicExprAfterType";"attributeTarget";"OBLOCKBEGIN";"oblockend"];"attributeList",["LBRACK_LESS";"attributeListElements";"opt_seps";"GREATER_RBRACK";"opt_OBLOCKSEP";"error";"ends_coming_soon_or_recover"];"attributeListElements",["attribute";"seps"];"attributeTarget",["moduleKeyword";"COLON";"typeKeyword";"ident";"YIELD"];"autoPropsDefnDecl",["VAL";"opt_mutable";"opt_access";"ident";"opt_typ";"EQUALS";"typedSequentialExprBlock";"classMemberSpfnGetSet"];"barAndgrabXmlDoc",["BAR"];"bar_rbrace",["BAR_RBRACE"];"baseSpec",["AS";"ident";"BASE"];"beginEndExpr",["BEGIN";"typedSequentialExpr";"END";"recover";"error"];"bindingPattern",["headBindingPattern"];"braceBarExpr",["STRUCT";"braceBarExprCore"];"braceBarExprCore",["LBRACE_BAR";"recdExprCore";"bar_rbrace";"recover";"error"];"braceBarFieldDeclListCore",["LBRACE_BAR";"recdFieldDeclList";"bar_rbrace";"recover";"error"];"braceExpr",["LBRACE";"braceExprBody";"rbrace";"recover";"error"];"braceExprBody",["recdExpr";"objExpr";"computationExpr"];"braceFieldDeclList",["LBRACE";"recdFieldDeclList";"rbrace";"recover";"error"];"cPrototype",["EXTERN";"cRetType";"opt_access";"ident";"opt_HIGH_PRECEDENCE_APP";"LPAREN";"externArgs";"rparen"];"cRetType",["opt_attributes";"cType";"VOID"];"cType",["path";"opt_HIGH_PRECEDENCE_APP";"LBRACK";"RBRACK";"STAR";"AMP";"VOID"];"classDefnBindings",["defnBindings";"doBinding";"hardwhiteLetBindings";"hardwhiteDoBinding"];"classDefnBlock",["OBLOCKBEGIN";"classDefnMembers";"recover";"oblockend"];"classDefnBlockKindUnspecified",["OBLOCKBEGIN";"classDefnMembers";"recover";"oblockend"];"classDefnMember",["opt_attributes";"opt_access";"classDefnBindings";"STATIC";"memberFlags";"memberCore";"opt_ODECLEND";"interfaceMember";"appType";"opt_interfaceImplDefn";"abstractMemberFlags";"opt_inline";"nameop";"opt_explicitValTyparDecls";"COLON";"topTypeWithTypeConstraints";"classMemberSpfnGetSet";"inheritsDefn";"valDefnDecl";"autoPropsDefnDecl";"NEW";"atomicPattern";"optAsSpec";"EQUALS";"typedSequentialExprBlock";"typeKeyword";"tyconDefn"];"classDefnMemberGetSet",["WITH";"classDefnMemberGetSetElements";"OWITH";"OEND";"error"];"classDefnMemberGetSetElement",["opt_inline";"opt_attributes";"bindingPattern";"opt_topReturnTypeWithTypeConstraints";"EQUALS";"typedSequentialExprBlock"];"classDefnMemberGetSetElements",["classDefnMemberGetSetElement";"AND"];"classDefnMembers",["classDefnMembersAtLeastOne";"error"];"classDefnMembersAtLeastOne",["classDefnMember";"opt_seps";"classDefnMembers"];"classMemberSpfn",["opt_attributes";"opt_access";"memberSpecFlags";"opt_inline";"nameop";"opt_explicitValTyparDecls";"COLON";"topTypeWithTypeConstraints";"classMemberSpfnGetSet";"optLiteralValueSpfn";"interfaceMember";"appType";"INHERIT";"VAL";"fieldDecl";"STATIC";"typeKeyword";"tyconSpfn";"NEW"];"classMemberSpfnGetSet",["WITH";"classMemberSpfnGetSetElements";"OWITH";"OEND";"error"];"classMemberSpfnGetSetElements",["nameop";"COMMA"];"classOrInterfaceOrStruct",["CLASS";"INTERFACE";"STRUCT"];"classSpfnBlock",["OBLOCKBEGIN";"classSpfnMembers";"oblockend";"recover"];"classSpfnBlockKindUnspecified",["OBLOCKBEGIN";"classSpfnMembers";"oblockend";"recover";"BEGIN";"classSpfnBlock";"END"];"classSpfnMembers",["classSpfnMembersAtLeastOne"];"classSpfnMembersAtLeastOne",["classMemberSpfn";"opt_seps";"classSpfnMembers"];"colonOrEquals",["COLON";"EQUALS"];"computationExpr",["sequentialExpr"];"conjParenPatternElements",["AMP";"parenPattern"];"conjPatternElements",["AMP";"headBindingPattern"];"constant",["rawConstant";"HIGH_PRECEDENCE_TYAPP";"measureTypeArg"];"constrPattern",["atomicPatternLongIdent";"explicitValTyparDecls";"atomicPatsOrNamePatPairs";"HIGH_PRECEDENCE_PAREN_APP";"HIGH_PRECEDENCE_BRACK_APP";"COLON_QMARK";"atomTypeOrAnonRecdType";"atomicPattern"];"declEnd",["ODECLEND";"OEND";"END"];"declExpr",["defnBindings";"IN";"typedSequentialExpr";"error";"hardwhiteLetBindings";"typedSequentialExprBlock";"OBLOCKSEP";"hardwhiteDoBinding";"anonMatchingExpr";"anonLambdaExpr";"MATCH";"withClauses";"recover";"MATCH_BANG";"TRY";"typedSequentialExprBlockR";"FINALLY";"IF";"ifExprCases";"LAZY";"ASSERT";"OLAZY";"declExprBlock";"OASSERT";"WHILE";"doToken";"doneDeclEnd";"FOR";"forLoopBinder";"ends_coming_soon_or_recover";"opt_OBLOCKSEP";"arrowThenExprR";"forLoopRange";"parenPattern";"YIELD";"YIELD_BANG";"BINDER";"headBindingPattern";"EQUALS";"moreBinders";"OBINDER";"hardwhiteDefnBindingsTerminator";"DO_BANG";"ODO_BANG";"FIXED";"RARROW";"COLON_QMARK";"typ";"COLON_GREATER";"COLON_QMARK_GREATER";"COLON_EQUALS";"minusExpr";"LARROW";"tupleExpr";"JOIN_IN";"BAR_BAR";"INFIX_BAR_OP";"OR";"AMP";"AMP_AMP";"INFIX_AMP_OP";"INFIX_COMPARE_OP";"DOLLAR";"LESS";"GREATER";"INFIX_AT_HAT_OP";"PERCENT_OP";"COLON_COLON";"PLUS_MINUS_OP";"MINUS";"STAR";"INFIX_STAR_DIV_MOD_OP";"INFIX_STAR_STAR_OP";"OBLOCKEND_COMING_SOON";"DOT_DOT"];"declExprBlock",["OBLOCKBEGIN";"typedSequentialExpr";"oblockend";"declExpr"];"deprecated_opt_equals",["EQUALS"];"doToken",["DO";"ODO"];"doneDeclEnd",["DONE";"ODECLEND"];"dummyTypeArg",[];"dynamicArg",["IDENT";"LPAREN";"typedSequentialExpr";"rparen"];"ends_coming_soon_or_recover",["TYPE_COMING_SOON";"MODULE_COMING_SOON";"RBRACE_COMING_SOON";"RPAREN_COMING_SOON";"OBLOCKEND_COMING_SOON";"recover"];"ends_other_than_rparen_coming_soon_or_recover",["TYPE_COMING_SOON";"MODULE_COMING_SOON";"RBRACE_COMING_SOON";"OBLOCKEND_COMING_SOON";"recover"];"exconCore",["EXCEPTION";"opt_attributes";"opt_access";"exconIntro";"exconRepr"];"exconIntro",["ident";"OF";"unionCaseRepr";"recover"];"exconRepr",["EQUALS";"path"];"exconSpfn",["exconCore";"opt_classSpfn"];"explicitValTyparDecls",["opt_HIGH_PRECEDENCE_TYAPP";"LESS";"explicitValTyparDeclsCore";"opt_typeConstraints";"GREATER"];"explicitValTyparDeclsCore",["typarDeclList";"COMMA";"DOT_DOT"];"externArg",["opt_attributes";"cType";"ident"];"externArgs",["externMoreArgs";"externArg"];"externMoreArgs",["COMMA";"externArg"];"fieldDecl",["opt_mutable";"opt_access";"ident";"COLON";"typ"];"fileModuleImpl",["opt_attributes";"opt_access";"moduleIntro";"moduleDefnsOrExprPossiblyEmptyOrBlock"];"fileModuleSpec",["opt_attributes";"opt_access";"moduleIntro";"moduleSpfnsPossiblyEmptyBlock"];"fileNamespaceImpl",["namespaceIntro";"deprecated_opt_equals";"fileModuleImpl"];"fileNamespaceImplList",["fileNamespaceImpl"];"fileNamespaceImpls",["fileModuleImpl";"fileNamespaceImplList"];"fileNamespaceSpec",["namespaceIntro";"deprecated_opt_equals";"fileModuleSpec"];"fileNamespaceSpecList",["fileNamespaceSpec"];"fileNamespaceSpecs",["fileModuleSpec";"fileNamespaceSpecList"];"firstUnionCaseDecl",["ident";"OF";"unionCaseRepr";"EQUALS";"constant";"opt_OBLOCKSEP"];"firstUnionCaseDeclOfMany",["ident";"opt_OBLOCKSEP";"EQUALS";"constant";"firstUnionCaseDecl"];"forLoopDirection",["TO";"DOWNTO"];"hardwhiteDefnBindingsTerminator",["ODECLEND";"recover"];"hardwhiteDoBinding",["ODO";"typedSequentialExprBlock";"hardwhiteDefnBindingsTerminator"];"hashDirectiveArg",["string";"sourceIdentifier"];"hashDirectiveArgs",["hashDirectiveArg"];"ident",["IDENT"];"identExpr",["ident";"opName"];"identOrOp",["ident";"opName"];"ifExprElifs",["ELSE";"declExpr";"OELSE";"OBLOCKBEGIN";"typedSequentialExpr";"oblockend";"recover";"ELIF";"ifExprCases"];"ifExprThen",["THEN";"declExpr";"OTHEN";"OBLOCKBEGIN";"typedSequentialExpr";"oblockend";"recover"];"implementationFile",["fileNamespaceImpls";"EOF";"error"];"inheritsDefn",["INHERIT";"atomTypeNonAtomicDeprecated";"optBaseSpec";"opt_HIGH_PRECEDENCE_APP";"atomicExprAfterType";"ends_coming_soon_or_recover"];"inlineAssemblyExpr",["HASH";"string";"opt_inlineAssemblyTypeArg";"optCurriedArgExprs";"optInlineAssemblyReturnTypes"];"interaction",["interactiveItemsTerminator";"SEMICOLON";"OBLOCKSEP"];"interactiveDefns",["moduleDefn"];"interactiveExpr",["opt_attributes";"opt_access";"declExpr"];"interactiveHash",["hashDirective"];"interactiveItemsTerminator",["interactiveTerminator";"interactiveDefns";"interactiveExpr";"interactiveHash";"interactiveSeparators"];"interactiveSeparator",["SEMICOLON";"OBLOCKSEP"];"interactiveSeparators",["interactiveSeparator"];"interactiveTerminator",["SEMICOLON_SEMICOLON";"EOF"];"interfaceMember",["INTERFACE";"OINTERFACE_MEMBER"];"interpolatedString",["INTERP_STRING_BEGIN_PART";"interpolatedStringFill";"interpolatedStringParts";"INTERP_STRING_BEGIN_END"];"interpolatedStringFill",["declExpr";"COLON";"ident"];"interpolatedStringParts",["INTERP_STRING_END";"INTERP_STRING_PART";"interpolatedStringFill"];"listExprElements",["sequentialExpr"];"listPatternElements",["parenPattern";"opt_seps";"seps"];"localBinding",["opt_inline";"opt_mutable";"bindingPattern";"opt_topReturnTypeWithTypeConstraints";"EQUALS";"typedExprWithStaticOptimizationsBlock";"error";"recover"];"localBindings",["attr_localBinding";"moreLocalBindings"];"measureTypeArg",["LESS";"measureTypeExpr";"GREATER";"UNDERSCORE"];"measureTypeAtom",["path";"typar";"LPAREN";"measureTypeExpr";"rparen"];"measureTypeExpr",["measureTypeSeq";"STAR";"INFIX_STAR_DIV_MOD_OP"];"measureTypePower",["measureTypeAtom";"INFIX_AT_HAT_OP";"atomicRationalConstant";"INT32"];"measureTypeSeq",["measureTypePower"];"memberCore",["opt_inline";"bindingPattern";"opt_topReturnTypeWithTypeConstraints";"EQUALS";"typedSequentialExprBlock";"classDefnMemberGetSet"];"memberFlags",["STATIC";"MEMBER";"OVERRIDE";"DEFAULT"];"memberSpecFlags",["memberFlags";"abstractMemberFlags"];"moduleDefn",["opt_attributes";"opt_access";"defnBindings";"hardwhiteLetBindings";"doBinding";"typeKeyword";"tyconDefn";"tyconDefnList";"exconDefn";"moduleIntro";"EQUALS";"namedModuleDefnBlock";"attributes";"recover";"openDecl"];"moduleDefnOrDirective",["moduleDefn";"hashDirective"];"moduleDefns",["moduleDefnOrDirective";"topSeparators";"moduleDefnsOrExpr";"error"];"moduleDefnsOrExpr",["opt_attributes";"opt_access";"declExpr";"topSeparators";"moduleDefns";"error"];"moduleDefnsOrExprPossiblyEmpty",["moduleDefnsOrExpr"];"moduleDefnsOrExprPossiblyEmptyOrBlock",["OBLOCKBEGIN";"moduleDefnsOrExprPossiblyEmpty";"oblockend";"opt_OBLOCKSEP";"recover";"error"];"moduleIntro",["moduleKeyword";"opt_attributes";"opt_access";"opt_rec";"path"];"moduleKeyword",["MODULE_COMING_SOON";"MODULE_IS_HERE";"MODULE"];"moduleSpecBlock",["OBLOCKBEGIN";"moduleSpfns";"oblockend";"sigOrBegin";"moduleSpfnsPossiblyEmpty";"END"];"moduleSpfn",["hashDirective";"valSpfn";"opt_attributes";"opt_access";"moduleIntro";"colonOrEquals";"namedModuleAbbrevBlock";"moduleSpecBlock";"typeKeyword";"tyconSpfn";"tyconSpfnList";"exconSpfn";"openDecl"];"moduleSpfns",["moduleSpfn";"opt_topSeparators";"error";"topSeparators"];"moduleSpfnsPossiblyEmpty",["moduleSpfns";"error"];
            "moduleSpfnsPossiblyEmptyBlock",["moduleSpfnsPossiblyEmpty";"OBLOCKBEGIN";"oblockend";"opt_OBLOCKSEP";"recover";"error"];"moreBinders",["AND_BANG";"headBindingPattern";"EQUALS";"typedSequentialExprBlock";"IN";"OAND_BANG";"hardwhiteDefnBindingsTerminator";"opt_OBLOCKSEP"];"moreLocalBindings",["AND";"attr_localBinding"];"namePatPair",["ident";"EQUALS";"parenPattern"];"namePatPairs",["namePatPair";"opt_seps";"seps"];"namedModuleAbbrevBlock",["OBLOCKBEGIN";"path";"oblockend"];"namedModuleDefnBlock",["OBLOCKBEGIN";"wrappedNamedModuleDefn";"oblockend";"recover";"moduleDefnsOrExpr";"error";"path"];"nameop",["identOrOp"];"namespaceIntro",["NAMESPACE";"opt_rec";"path"];"objExpr",["objExprBaseCall";"objExprBindings";"opt_OBLOCKSEP";"opt_objExprInterfaces";"objExprInterfaces";"NEW";"atomTypeNonAtomicDeprecated"];"objExprBaseCall",["NEW";"atomTypeNonAtomicDeprecated";"opt_HIGH_PRECEDENCE_APP";"atomicExprAfterType";"baseSpec"];"objExprBindings",["WITH";"localBindings";"OWITH";"OEND";"objectImplementationBlock";"opt_declEnd"];"objExprInterface",["interfaceMember";"appType";"opt_objExprBindings";"opt_declEnd";"opt_OBLOCKSEP"];"objExprInterfaces",["objExprInterface";"opt_objExprInterfaces"];"objectImplementationBlock",["OBLOCKBEGIN";"objectImplementationMembers";"oblockend";"recover"];"objectImplementationMember",["opt_attributes";"staticMemberOrMemberOrOverride";"memberCore";"opt_ODECLEND";"autoPropsDefnDecl";"error"];"objectImplementationMembers",["objectImplementationMember";"opt_seps"];"oblockend",["OBLOCKEND_COMING_SOON";"OBLOCKEND_IS_HERE";"OBLOCKEND"];"opName",["LPAREN";"operatorName";"rparen";"error";"LPAREN_STAR_RPAREN";"activePatternCaseNames";"BAR";"UNDERSCORE"];"openDecl",["OPEN";"path";"typeKeyword";"appType"];"operatorName",["PREFIX_OP";"INFIX_STAR_STAR_OP";"INFIX_COMPARE_OP";"INFIX_AT_HAT_OP";"INFIX_BAR_OP";"INFIX_AMP_OP";"PLUS_MINUS_OP";"INFIX_STAR_DIV_MOD_OP";"DOLLAR";"ADJACENT_PREFIX_OP";"MINUS";"STAR";"EQUALS";"OR";"LESS";"GREATER";"QMARK";"AMP";"AMP_AMP";"BAR_BAR";"COLON_EQUALS";"FUNKY_OPERATOR_NAME";"PERCENT_OP";"DOT_DOT";"LQUOTE";"RQUOTE"];"optAsSpec",["asSpec"];"optBaseSpec",["baseSpec"];"optCurriedArgExprs",["argExpr"];"optInlineAssemblyReturnTypes",["COLON";"typ";"LPAREN";"rparen"];"optLiteralValueSpfn",["EQUALS";"declExpr";"OBLOCKBEGIN";"oblockend";"opt_ODECLEND"];"opt_HIGH_PRECEDENCE_APP",["HIGH_PRECEDENCE_BRACK_APP";"HIGH_PRECEDENCE_PAREN_APP"];"opt_HIGH_PRECEDENCE_TYAPP",["HIGH_PRECEDENCE_TYAPP"];"opt_OBLOCKSEP",["OBLOCKSEP"];"opt_ODECLEND",["ODECLEND"];"opt_access",["access"];"opt_atomicExprAfterType",["atomicExprAfterType"];"opt_attributes",["attributes"];"opt_bar",["BAR"];"opt_classDefn",["WITH";"classDefnBlock";"declEnd"];"opt_classSpfn",["WITH";"classSpfnBlock";"declEnd"];"opt_declEnd",["ODECLEND";"OEND";"END"];"opt_equals",["EQUALS"];"opt_explicitValTyparDecls",["explicitValTyparDecls"];"opt_inline",["INLINE"];"opt_inlineAssemblyTypeArg",["typeKeyword";"LPAREN";"typ";"rparen"];"opt_interfaceImplDefn",["WITH";"objectImplementationBlock";"declEnd"];"opt_mutable",["MUTABLE"];"opt_objExprBindings",["objExprBindings"];"opt_objExprInterfaces",["objExprInterface";"error"];"opt_rec",["REC"];"opt_seps",["seps"];"opt_seps_recd",["seps_recd"];"opt_staticOptimizations",["staticOptimization"];"opt_topReturnTypeWithTypeConstraints",["COLON";"topTypeWithTypeConstraints"];"opt_topSeparators",["topSeparator"];"opt_typ",["COLON";"typ"];"opt_typeConstraints",["WHEN";"typeConstraints"];"parenExpr",["LPAREN";"rparen";"parenExprBody";"ends_other_than_rparen_coming_soon_or_recover";"error";"TYPE_COMING_SOON";"MODULE_COMING_SOON";"RBRACE_COMING_SOON";"OBLOCKEND_COMING_SOON";"recover"];"parenExprBody",["typars";"COLON";"LPAREN";"classMemberSpfn";"rparen";"typedSequentialExpr";"inlineAssemblyExpr"];"parenPattern",["AS";"constrPattern";"BAR";"tupleParenPatternElements";"conjParenPatternElements";"COLON";"typeWithTypeConstraints";"attributes";"COLON_COLON"];"parenPatternBody",["parenPattern"];"path",["GLOBAL";"ident";"DOT";"ends_coming_soon_or_recover"];"pathOp",["ident";"opName";"DOT";"error"];"pathOrUnderscore",["path";"UNDERSCORE"];"patternAndGuard",["parenPattern";"patternGuard"];"patternClauses",["patternAndGuard";"patternResult";"BAR";"error"];"patternGuard",["WHEN";"declExpr"];"patternResult",["RARROW";"typedSequentialExprBlockR"];"postfixTyparDecls",["opt_HIGH_PRECEDENCE_TYAPP";"LESS";"typarDeclList";"opt_typeConstraints";"GREATER"];"powerType",["atomTypeOrAnonRecdType";"INFIX_AT_HAT_OP";"atomicRationalConstant"];"prefixTyparDecls",["typar";"LPAREN";"typarDeclList";"rparen"];"quoteExpr",["LQUOTE";"typedSequentialExpr";"RQUOTE";"recover";"error"];"rationalConstant",["INT32";"INFIX_STAR_DIV_MOD_OP";"MINUS"];"rawConstant",["INT8";"UINT8";"INT16";"UINT16";"INT32";"UINT32";"INT64";"UINT64";"NATIVEINT";"UNATIVEINT";"IEEE32";"IEEE64";"CHAR";"DECIMAL";"BIGNUM";"string";"sourceIdentifier";"BYTEARRAY"];"rbrace",["RBRACE_COMING_SOON";"RBRACE_IS_HERE";"RBRACE"];"recdBinding",["pathOrUnderscore";"EQUALS";"declExprBlock";"ends_coming_soon_or_recover"];"recdExpr",["INHERIT";"atomTypeNonAtomicDeprecated";"opt_HIGH_PRECEDENCE_APP";"opt_atomicExprAfterType";"recdExprBindings";"opt_seps_recd";"recdExprCore"];"recdExprBindings",["seps_recd";"recdBinding"];"recdExprCore",["appExpr";"EQUALS";"declExprBlock";"recdExprBindings";"opt_seps_recd";"UNDERSCORE";"WITH";"recdBinding";"OWITH";"OEND"];"recdFieldDecl",["opt_attributes";"fieldDecl"];"recdFieldDeclList",["recdFieldDecl";"seps";"opt_seps"];"recordPatternElement",["path";"EQUALS";"parenPattern"];"recordPatternElementsAux",["recordPatternElement";"opt_seps";"seps"];"recover",["error";"EOF"];"rparen",["RPAREN_COMING_SOON";"RPAREN_IS_HERE";"RPAREN"];"seps",["OBLOCKSEP";"SEMICOLON"];"seps_recd",["OBLOCKSEP";"SEMICOLON"];"sigOrBegin",["SIG";"BEGIN"];"signatureFile",["fileNamespaceSpecs";"EOF";"error"];"simplePattern",["ident";"QMARK";"COLON";"typeWithTypeConstraints";"attributes"];"simplePatternCommaList",["simplePattern";"COMMA"];"simplePatterns",["LPAREN";"simplePatternCommaList";"rparen";"recover";"error"];"sourceIdentifier",["KEYWORD_STRING"];"staticMemberOrMemberOrOverride",["STATIC";"MEMBER";"OVERRIDE"];"staticOptimization",["WHEN";"staticOptimizationConditions";"EQUALS";"typedSequentialExprBlock"];"staticOptimizationCondition",["typar";"COLON";"typ";"STRUCT"];"staticOptimizationConditions",["AND";"staticOptimizationCondition"];"string",["STRING"];"structOrBegin",["STRUCT";"BEGIN"];"topAppType",["attributes";"appType";"COLON";"QMARK";"ident"];"topSeparator",["SEMICOLON";"SEMICOLON_SEMICOLON";"OBLOCKSEP"];"topSeparators",["topSeparator"];"topTupleType",["topAppType";"STAR";"topTupleTypeElements"];"topTupleTypeElements",["topAppType";"STAR"];"topType",["topTupleType";"RARROW"];"topTypeWithTypeConstraints",["topType";"WHEN";"typeConstraints"];"tupleExpr",["COMMA";"declExpr";"ends_coming_soon_or_recover"];"tupleOrQuotTypeElements",["appType";"STAR";"INFIX_STAR_DIV_MOD_OP"];"tupleParenPatternElements",["COMMA";"parenPattern";"ends_coming_soon_or_recover"];"tuplePatternElements",["COMMA";"headBindingPattern";"ends_coming_soon_or_recover"];"tupleType",["appType";"STAR";"tupleOrQuotTypeElements";"INFIX_STAR_DIV_MOD_OP"];"tyconClassDefn",["classDefnBlockKindUnspecified";"classOrInterfaceOrStruct";"classDefnBlock";"END";"recover";"error"];"tyconClassSpfn",["classSpfnBlockKindUnspecified";"classOrInterfaceOrStruct";"classSpfnBlock";"END";"recover";"error"];"tyconDefnAugmentation",["WITH";"classDefnBlock";"declEnd"];"tyconDefnOrSpfnSimpleRepr",["opt_attributes";"opt_access";"path";"LQUOTE";"STRING";"recover";"typ";"unionTypeRepr";"braceFieldDeclList";"LPAREN";"HASH";"string";"rparen"];"tyconDefnRhs",["tyconDefnOrSpfnSimpleRepr";"tyconClassDefn";"DELEGATE";"OF";"topType"];"tyconDefnRhsBlock",["OBLOCKBEGIN";"tyconDefnRhs";"opt_OBLOCKSEP";"classDefnMembers";"opt_classDefn";"oblockend";"recover"];"tyconNameAndTyparDecls",["opt_access";"path";"prefixTyparDecls";"postfixTyparDecls"];"tyconSpfn",["typeNameInfo";"EQUALS";"tyconSpfnRhsBlock";"opt_classSpfn"];"tyconSpfnList",["AND";"tyconSpfn"];"tyconSpfnRhs",["tyconDefnOrSpfnSimpleRepr";"tyconClassSpfn";"DELEGATE";"OF";"topType"];"tyconSpfnRhsBlock",["OBLOCKBEGIN";"tyconSpfnRhs";"opt_OBLOCKSEP";"classSpfnMembers";"opt_classSpfn";"oblockend"];"typEOF",["typ";"EOF"];"typar",["QUOTE";"ident";"INFIX_AT_HAT_OP"];"typarAlts",["OR";"appType";"typar"];"typarDecl",["opt_attributes";"typar"];"typarDeclList",["COMMA";"typarDecl"];"typars",["typar";"LPAREN";"typarAlts";"rparen"];"typeAlts",["OR";"appType"];"typeArgActual",["typ";"EQUALS"];"typeArgActualOrDummyIfEmpty",["typeArgActual";"dummyTypeArg"];"typeArgListElements",["COMMA";"typeArgActual";"dummyTypeArg"];"typeArgsActual",["LESS";"typeArgActualOrDummyIfEmpty";"COMMA";"typeArgListElements";"GREATER";"recover";"ends_coming_soon_or_recover";"typeArgActual"];"typeArgsNoHpaDeprecated",["typeArgsActual";"HIGH_PRECEDENCE_TYAPP"];"typeConstraints",["AND";"typeConstraint"];"typeKeyword",["TYPE_COMING_SOON";"TYPE_IS_HERE";"TYPE"];"typeNameInfo",["opt_attributes";"tyconNameAndTyparDecls";"opt_typeConstraints"];"typeWithTypeConstraints",["typ";"WHEN";"typeConstraints"];"typedExprWithStaticOptimizations",["typedSequentialExpr";"opt_staticOptimizations"];"typedExprWithStaticOptimizationsBlock",["OBLOCKBEGIN";"typedExprWithStaticOptimizations";"oblockend";"recover"];"typedSequentialExpr",["sequentialExpr";"COLON";"typeWithTypeConstraints"];"typedSequentialExprBlock",["OBLOCKBEGIN";"typedSequentialExpr";"oblockend";"recover"];"typedSequentialExprBlockR",["typedSequentialExpr";"ORIGHT_BLOCK_END"];"typedSequentialExprEOF",["typedSequentialExpr";"EOF"];"unionCaseName",["nameop";"LPAREN";"COLON_COLON";"rparen";"LBRACK";"RBRACK"];"unionCaseRepr",["braceFieldDeclList";"unionCaseReprElements"];"unionCaseReprElement",["ident";"COLON";"appType"];"unionCaseReprElements",["unionCaseReprElement";"STAR"];"unionTypeRepr",["barAndgrabXmlDoc";"attrUnionCaseDecls";"firstUnionCaseDeclOfMany";"firstUnionCaseDecl"];"valDefnDecl",["VAL";"opt_mutable";"opt_access";"ident";"COLON";"typ"];"valSpfn",["opt_attributes";"opt_access";"VAL";"opt_inline";"opt_mutable";"nameop";"opt_explicitValTyparDecls";"COLON";"topTypeWithTypeConstraints";"optLiteralValueSpfn"];"withPatternClauses",["patternClauses";"BAR";"error"];"wrappedNamedModuleDefn",["structOrBegin";"moduleDefnsOrExprPossiblyEmpty";"END";"recover";"error"]]
        let s0 = "implementationFile"

        let y = List.depthFirstSort nodes s0
        show y
        //注意：结果列表忽略无关的元素
        let e = ["implementationFile";"fileNamespaceImpls";"fileModuleImpl";"opt_attributes";"attributes";"opt_access";"access";"PRIVATE";"PUBLIC";"INTERNAL";"moduleIntro";"moduleKeyword";"MODULE_COMING_SOON";"MODULE_IS_HERE";"MODULE";"opt_rec";"REC";"path";"GLOBAL";"ident";"IDENT";"DOT";"ends_coming_soon_or_recover";"TYPE_COMING_SOON";"RBRACE_COMING_SOON";"RPAREN_COMING_SOON";"OBLOCKEND_COMING_SOON";"recover";"error";"EOF";"moduleDefnsOrExprPossiblyEmptyOrBlock";"OBLOCKBEGIN";"moduleDefnsOrExprPossiblyEmpty";"moduleDefnsOrExpr";"declExpr";"defnBindings";"IN";"typedSequentialExpr";"sequentialExpr";"COLON";"typeWithTypeConstraints";"typ";"WHEN";"typeConstraints";"AND";"typeConstraint";"hardwhiteLetBindings";"typedSequentialExprBlock";"oblockend";"OBLOCKEND_IS_HERE";"OBLOCKEND";"OBLOCKSEP";"hardwhiteDoBinding";"ODO";"hardwhiteDefnBindingsTerminator";"ODECLEND";"anonMatchingExpr";"anonLambdaExpr";"MATCH";"withClauses";"MATCH_BANG";"TRY";"typedSequentialExprBlockR";"ORIGHT_BLOCK_END";"FINALLY";"IF";"ifExprCases";"LAZY";"ASSERT";"OLAZY";"declExprBlock";"OASSERT";"WHILE";"doToken";"DO";"doneDeclEnd";"DONE";"FOR";"forLoopBinder";"opt_OBLOCKSEP";"arrowThenExprR";"RARROW";"forLoopRange";"parenPattern";"AS";"constrPattern";"atomicPatternLongIdent";"UNDERSCORE";"pathOp";"opName";"LPAREN";"operatorName";"PREFIX_OP";"INFIX_STAR_STAR_OP";"INFIX_COMPARE_OP";"INFIX_AT_HAT_OP";"INFIX_BAR_OP";"INFIX_AMP_OP";"PLUS_MINUS_OP";"INFIX_STAR_DIV_MOD_OP";"DOLLAR";"ADJACENT_PREFIX_OP";"MINUS";"STAR";"EQUALS";"OR";"LESS";"GREATER";"QMARK";"AMP";"AMP_AMP";"BAR_BAR";"COLON_EQUALS";"FUNKY_OPERATOR_NAME";"PERCENT_OP";"DOT_DOT";"LQUOTE";"RQUOTE";"rparen";"RPAREN_IS_HERE";"RPAREN";"LPAREN_STAR_RPAREN";"activePatternCaseNames";"BAR";"activePatternCaseName";"explicitValTyparDecls";"opt_HIGH_PRECEDENCE_TYAPP";"HIGH_PRECEDENCE_TYAPP";"explicitValTyparDeclsCore";"typarDeclList";"COMMA";"typarDecl";"typar";"QUOTE";"opt_typeConstraints";"atomicPatsOrNamePatPairs";"namePatPairs";"namePatPair";"opt_seps";"seps";"SEMICOLON";"atomicPatterns";"atomicPattern";"quoteExpr";"CHAR";"LBRACE";"recordPatternElementsAux";"recordPatternElement";"rbrace";"RBRACE_IS_HERE";"RBRACE";"LBRACK";"listPatternElements";"RBRACK";"LBRACK_BAR";"BAR_RBRACK";"constant";"rawConstant";"INT8";"UINT8";"INT16";"UINT16";"INT32";"UINT32";"INT64";"UINT64";"NATIVEINT";"UNATIVEINT";"IEEE32";"IEEE64";"DECIMAL";"BIGNUM";"string";"STRING";"sourceIdentifier";"KEYWORD_STRING";"BYTEARRAY";"measureTypeArg";"measureTypeExpr";"measureTypeSeq";"measureTypePower";"measureTypeAtom";"atomicRationalConstant";"atomicUnsignedRationalConstant";"rationalConstant";"FALSE";"TRUE";"NULL";"parenPatternBody";"STRUCT";"tupleParenPatternElements";"HIGH_PRECEDENCE_BRACK_APP";"HIGH_PRECEDENCE_PAREN_APP";"COLON_QMARK";"atomTypeOrAnonRecdType";"atomType";"HASH";"appTypeConPower";"appTypeCon";"appType";"tupleOrQuotTypeElements";"CONST";"atomicExpr";"typeArgsActual";"typeArgActualOrDummyIfEmpty";"typeArgActual";"dummyTypeArg";"typeArgListElements";"atomicExprQualification";"identOrOp";"COLON_COLON";"BASE";"nameop";"dynamicArg";"identExpr";"listExprElements";"tupleExpr";"atomicExprAfterType";"parenExpr";"parenExprBody";"typars";"typarAlts";"classMemberSpfn";"memberSpecFlags";"memberFlags";"STATIC";"MEMBER";"OVERRIDE";"DEFAULT";"abstractMemberFlags";"ABSTRACT";"opt_inline";"INLINE";"opt_explicitValTyparDecls";"topTypeWithTypeConstraints";"topType";"topTupleType";"topAppType";"topTupleTypeElements";"classMemberSpfnGetSet";"WITH";"classMemberSpfnGetSetElements";"OWITH";"OEND";"optLiteralValueSpfn";"opt_ODECLEND";"interfaceMember";"INTERFACE";"OINTERFACE_MEMBER";"INHERIT";"VAL";"fieldDecl";"opt_mutable";"MUTABLE";"typeKeyword";"TYPE_IS_HERE";"TYPE";"tyconSpfn";"typeNameInfo";"tyconNameAndTyparDecls";"prefixTyparDecls";"postfixTyparDecls";"tyconSpfnRhsBlock";"tyconSpfnRhs";"tyconDefnOrSpfnSimpleRepr";"unionTypeRepr";"barAndgrabXmlDoc";"attrUnionCaseDecls";"attrUnionCaseDecl";"unionCaseName";"OF";"unionCaseRepr";"braceFieldDeclList";"recdFieldDeclList";"recdFieldDecl";"unionCaseReprElements";"unionCaseReprElement";"firstUnionCaseDeclOfMany";"firstUnionCaseDecl";"tyconClassSpfn";"classSpfnBlockKindUnspecified";"classSpfnMembers";"classSpfnMembersAtLeastOne";"BEGIN";"classSpfnBlock";"END";"classOrInterfaceOrStruct";"CLASS";"DELEGATE";"opt_classSpfn";"declEnd";"NEW";"inlineAssemblyExpr";"opt_inlineAssemblyTypeArg";"optCurriedArgExprs";"argExpr";"optInlineAssemblyReturnTypes";"ends_other_than_rparen_coming_soon_or_recover";"braceExpr";"braceExprBody";"recdExpr";"atomTypeNonAtomicDeprecated";"appTypePrefixArguments";"opt_HIGH_PRECEDENCE_APP";"opt_atomicExprAfterType";"recdExprBindings";"seps_recd";"recdBinding";"pathOrUnderscore";"opt_seps_recd";"recdExprCore";"appExpr";"objExpr";"objExprBaseCall";"baseSpec";"objExprBindings";"localBindings";"attr_localBinding";"localBinding";"bindingPattern";"headBindingPattern";"opt_topReturnTypeWithTypeConstraints";"typedExprWithStaticOptimizationsBlock";"typedExprWithStaticOptimizations";"opt_staticOptimizations";"staticOptimization";"staticOptimizationConditions";"staticOptimizationCondition";"moreLocalBindings";"objectImplementationBlock";"objectImplementationMembers";"objectImplementationMember";"staticMemberOrMemberOrOverride";"memberCore";"classDefnMemberGetSet";"classDefnMemberGetSetElements";"classDefnMemberGetSetElement";"autoPropsDefnDecl";"opt_typ";"opt_declEnd";"opt_objExprInterfaces";"objExprInterface";"opt_objExprBindings";"objExprInterfaces";"computationExpr";"braceBarExpr";"braceBarExprCore";"LBRACE_BAR";"bar_rbrace";"BAR_RBRACE";"interpolatedString";"INTERP_STRING_BEGIN_PART";"interpolatedStringFill";"interpolatedStringParts";"INTERP_STRING_END";"INTERP_STRING_PART";"INTERP_STRING_BEGIN_END";"arrayExpr";"arrayExprElements";"beginEndExpr";"typeArgsNoHpaDeprecated";"anonRecdType";"braceBarFieldDeclListCore";"conjParenPatternElements";"YIELD";"YIELD_BANG";"BINDER";"moreBinders";"AND_BANG";"OAND_BANG";"OBINDER";"DO_BANG";"ODO_BANG";"FIXED";"COLON_GREATER";"COLON_QMARK_GREATER";"minusExpr";"LARROW";"JOIN_IN";"topSeparators";"topSeparator";"SEMICOLON_SEMICOLON";"moduleDefns";"moduleDefnOrDirective";"moduleDefn";"doBinding";"tyconDefn";"tyconDefnList";"exconDefn";"namedModuleDefnBlock";"wrappedNamedModuleDefn";"structOrBegin";"openDecl";"OPEN";"hashDirective";"fileNamespaceImplList";"fileNamespaceImpl";"namespaceIntro";"NAMESPACE";"deprecated_opt_equals"]

        Should.equal e y
        ()

    [<Fact>]
    member this.``empty countRev``() =
        let xs = []
        
        let count,ys = 
            xs
            |> List.countRev

        Should.equal count 0
        Should.equal ys xs

    [<Fact>]
    member this.``proper countRev``() =
        let xs = [5;4;3;2;1]
        
        let count,ys = 
            xs
            |> List.countRev

        let es = [1;2;3;4;5]
        Should.equal count 5
        Should.equal ys es


    [<Fact>]
    member _.``empty test`` () =
        let y = List.splitBy "x" []
        Should.equal [] y

    [<Fact>]
    member _.``splitBy noexists test`` () =
        let y = List.splitBy "x" ["y"]
        Should.equal [["y"]] y

    [<Fact>]
    member _.``splitBy noexists 2 test`` () =
        let y = List.splitBy "x" ["a";"y"]
        Should.equal [["a";"y"]] y

    [<Fact>]
    member _.``splitBy exists 1 test`` () =
        let y = List.splitBy "x" ["x"]
        Should.equal [["x"]] y

    [<Fact>]
    member _.``splitBy exists abc a test`` () =
        let y = List.splitBy "a" ["a";"b";"c"]
        Should.equal [["a"];["b";"c"]] y

    [<Fact>]
    member _.``splitBy exists abc b test`` () =
        let y = List.splitBy "b" ["a";"b";"c"]
        Should.equal [["a"];["b"];["c"]] y

    [<Fact>]
    member _.``splitBy exists abc c test`` () =
        let y = List.splitBy "c" ["a";"b";"c"]
        Should.equal [["a";"b"];["c"]] y

    [<Fact>]
    member _.``splitBy exists abccb b test`` () =
        let y = List.splitBy "b" ["a";"b";"c";"c";"b"]
        Should.equal [["a"];["b"];["c";"c"];["b"]] y

    [<Fact>]
    member _.``splitBy exists abccb c test`` () =
        let y = List.splitBy "c" ["a";"b";"c";"c";"b"]
        Should.equal [["a";"b"];["c"];["c"];["b"]] y

    [<Fact>]
    member _.``crosspower abcd test`` () =
        let a = [["a";"b"];["c";"d"];]

        let y1 = List.crosspower 1 a
        let e1 = [
            [["a";"b"]]
            [["c";"d"]]
            ]

        Should.equal e1 y1

        let y2 = List.crosspower 2 a
        let e2 = [
            [["a";"b"];["a";"b"]]
            [["a";"b"];["c";"d"]]
            [["c";"d"];["a";"b"]]
            [["c";"d"];["c";"d"]]
            ]

        let y3 = List.crosspower 3 a
        let e3 =[
            [["a";"b"];["a";"b"];["a";"b"]];
            [["a";"b"];["a";"b"];["c";"d"]];
            [["a";"b"];["c";"d"];["a";"b"]];
            [["a";"b"];["c";"d"];["c";"d"]];
            [["c";"d"];["a";"b"];["a";"b"]];
            [["c";"d"];["a";"b"];["c";"d"]];
            [["c";"d"];["c";"d"];["a";"b"]];
            [["c";"d"];["c";"d"];["c";"d"]]]

        show y3
        Should.equal e3 y3

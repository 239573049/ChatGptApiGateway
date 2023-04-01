import * as signalR from "@microsoft/signalr";
import { Component } from "react";

import * as monaco from 'monaco-editor';

monaco.languages.register({ id: "mySpecialLanguage" });

// Register a tokens provider for the language
monaco.languages.setMonarchTokensProvider("mySpecialLanguage", {
    tokenizer: {
        root: [
            [/\[Error.*/, "custom-error"],
            [/\[notice.*/, "custom-notice"],
            [/\[Information.*/, "custom-info"],
            [/\[[a-zA-Z 0-9:]+\]/, "custom-date"],
        ],
    },
});

// Define a new theme that contains only rules that match this language
monaco.editor.defineTheme("myCoolTheme", {
    base: "vs",
    inherit: false,
    rules: [
        { token: "custom-info", foreground: "808080" },
        { token: "custom-error", foreground: "ff0000", fontStyle: "bold" },
        { token: "custom-notice", foreground: "FFA500" },
        { token: "custom-date", foreground: "008800" },
    ],
    colors: {
        "editor.foreground": "#000000",
    },
});

// Register a completion item provider for the new language
monaco.languages.registerCompletionItemProvider("mySpecialLanguage", {
    provideCompletionItems: (model, position) => {
        var word = model.getWordUntilPosition(position);
        var range = {
            startLineNumber: position.lineNumber,
            endLineNumber: position.lineNumber,
            startColumn: word.startColumn,
            endColumn: word.endColumn,
        };
        var suggestions = [
            {
                label: "simpleText",
                kind: monaco.languages.CompletionItemKind.Text,
                insertText: "simpleText",
                range: range,
            },
            {
                label: "testing",
                kind: monaco.languages.CompletionItemKind.Keyword,
                insertText: "testing(${1:condition})",
                insertTextRules:
                    monaco.languages.CompletionItemInsertTextRule
                        .InsertAsSnippet,
                range: range,
            },
            {
                label: "ifelse",
                kind: monaco.languages.CompletionItemKind.Snippet,
                insertText: [
                    "if (${1:condition}) {",
                    "\t$0",
                    "} else {",
                    "\t",
                    "}",
                ].join("\n"),
                insertTextRules:
                    monaco.languages.CompletionItemInsertTextRule
                        .InsertAsSnippet,
                documentation: "If-Else Statement",
                range: range,
            },
        ];
        return { suggestions: suggestions };
    },
});

interface IProps {

}

interface IState {
    editor: monaco.editor.IStandaloneCodeEditor | null,
    init: boolean,
    hubConnection: signalR.HubConnection | null,
    value: string,
    timerID: any
}
export default class Logger extends Component<IProps, IState>{

    state: IState = {
        editor: null,
        init: false,
        hubConnection: null,
        value: "",
        timerID: undefined
    }
    getLogger() {
        try {
            var { hubConnection } = this.state;
            hubConnection?.send('GetLogger','logger')
        } catch (e) {
            console.log(e)
        }
    }

    componentDidMount() {
        var { editor, init } = this.state;
        
        var hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(process.env.NODE_ENV === "development" ? "http://localhost:53969/logger-hub" : "/logger-hub", { accessTokenFactory: () => window.localStorage.getItem("token")! })
            .build()

        hubConnection.start();

        this.setState({
            hubConnection,
            timerID: setInterval(() => this.getLogger(), 1000)
        })

        hubConnection.on('logger', (data: string) => {
            var { value, editor } = this.state;
            value += data + "\r\n";
            editor?.setValue(value);
            this.setState({
                value
            })
        });

        this.state.editor?.dispose();
        if (init === false) {
            var container = document.getElementById("container")!;
            container.innerText = '';
            editor = monaco.editor.create(container,
                {
                    theme: "myCoolTheme",
                    value: '',
                    language: "mySpecialLanguage",
                }
            )
        }
        this.setState({
            editor,
            hubConnection,
            timerID: setInterval(() => this.getLogger(), 1000),
            init: true
        })
    }

    componentWillUnmount() {
        this.state.editor?.dispose();
        this.state.hubConnection?.stop();
    }
    render() {
        return (
            <div id="container" style={{ height: "100%" }}>
            </div>)
    }
}
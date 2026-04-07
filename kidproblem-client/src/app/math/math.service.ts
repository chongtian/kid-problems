import { Injectable } from "@angular/core";
import { Subject, ReplaySubject, Observable } from "rxjs";

interface MathJaxConfig {
    source: string;
    id: string;
    customConfiguration?: string;
}

declare global {
    interface Window {
        MathJax: {
            typesetPromise: () => void;
            startup: {
                promise: Promise<any>;
            };
        };
    }
}

@Injectable({
    providedIn: "root"
})
export class MathService {
    private signal: Subject<boolean>;
    private mathJax: MathJaxConfig = {
        source: 'https://cdn.jsdelivr.net/npm/mathjax@4/startup.js',
        id: 'MathJaxScript',
        customConfiguration: `
        MathJax = {
            "loader": {
                "load": ["output/svg", "[tex]/require", "[tex]/ams", "[tex]/physics"]
              },
              "tex": {
                inlineMath: [["$", "$"],  ["\\\\[", "\\\\]"]],
                displayMath: [["$$", "$$"]],
                "packages": ["base", "require", "ams"] 
              },
              "svg": { "fontCache": "global" }
          };
          `
    };

    constructor() {
        this.signal = new ReplaySubject<boolean>();
        void this.registerMathJaxAsync(this.mathJax)
            .then(() => this.signal.next())
            .catch(error => {
                console.log(error);
            });
    }

    private async registerMathJaxAsync(config: MathJaxConfig): Promise<any> {
        return new Promise<void>((resolve, reject) => {
            const script: HTMLScriptElement = document.createElement("script");
            script.id = config.id;
            script.type = "text/javascript";
            script.src = config.source;
            script.crossOrigin = "anonymous";
            script.async = true;
            script.onload = () => resolve();
            script.onerror = error => reject(error);
            document.head.appendChild(script);

            const mathConfig: HTMLScriptElement = document.createElement('script')
            mathConfig.id = config.id + "Config";
            mathConfig.type = 'text/javascript';
            mathConfig.text = config.customConfiguration;
            document.head.appendChild(mathConfig);
        });
    }

    ready(): Observable<boolean> {
        return this.signal;
    }

    render(element: HTMLElement, math: string) {
        // Take initial typesetting which MathJax performs into account
        window.MathJax.startup.promise.then(() => {
            element.innerHTML = math;
            window.MathJax.typesetPromise();
        });
    }
}
"use strict";(self.webpackChunkdocs=self.webpackChunkdocs||[]).push([[495],{8570:(e,t,r)=>{r.d(t,{Zo:()=>s,kt:()=>k});var n=r(79);function a(e,t,r){return t in e?Object.defineProperty(e,t,{value:r,enumerable:!0,configurable:!0,writable:!0}):e[t]=r,e}function i(e,t){var r=Object.keys(e);if(Object.getOwnPropertySymbols){var n=Object.getOwnPropertySymbols(e);t&&(n=n.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),r.push.apply(r,n)}return r}function o(e){for(var t=1;t<arguments.length;t++){var r=null!=arguments[t]?arguments[t]:{};t%2?i(Object(r),!0).forEach((function(t){a(e,t,r[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(r)):i(Object(r)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(r,t))}))}return e}function l(e,t){if(null==e)return{};var r,n,a=function(e,t){if(null==e)return{};var r,n,a={},i=Object.keys(e);for(n=0;n<i.length;n++)r=i[n],t.indexOf(r)>=0||(a[r]=e[r]);return a}(e,t);if(Object.getOwnPropertySymbols){var i=Object.getOwnPropertySymbols(e);for(n=0;n<i.length;n++)r=i[n],t.indexOf(r)>=0||Object.prototype.propertyIsEnumerable.call(e,r)&&(a[r]=e[r])}return a}var p=n.createContext({}),c=function(e){var t=n.useContext(p),r=t;return e&&(r="function"==typeof e?e(t):o(o({},t),e)),r},s=function(e){var t=c(e.components);return n.createElement(p.Provider,{value:t},e.children)},u="mdxType",d={inlineCode:"code",wrapper:function(e){var t=e.children;return n.createElement(n.Fragment,{},t)}},m=n.forwardRef((function(e,t){var r=e.components,a=e.mdxType,i=e.originalType,p=e.parentName,s=l(e,["components","mdxType","originalType","parentName"]),u=c(r),m=a,k=u["".concat(p,".").concat(m)]||u[m]||d[m]||i;return r?n.createElement(k,o(o({ref:t},s),{},{components:r})):n.createElement(k,o({ref:t},s))}));function k(e,t){var r=arguments,a=t&&t.mdxType;if("string"==typeof e||a){var i=r.length,o=new Array(i);o[0]=m;var l={};for(var p in t)hasOwnProperty.call(t,p)&&(l[p]=t[p]);l.originalType=e,l[u]="string"==typeof e?e:a,o[1]=l;for(var c=2;c<i;c++)o[c]=r[c];return n.createElement.apply(null,o)}return n.createElement.apply(null,r)}m.displayName="MDXCreateElement"},3581:(e,t,r)=>{r.r(t),r.d(t,{assets:()=>p,contentTitle:()=>o,default:()=>d,frontMatter:()=>i,metadata:()=>l,toc:()=>c});var n=r(5675),a=(r(79),r(8570));const i={sidebar_position:1},o="Overview",l={unversionedId:"dev/directory/index",id:"dev/directory/index",title:"Overview",description:"The Directory project is the core piece of the BiobankingUK stack. Everything else in the stack serves to optionally augment the Directory.",source:"@site/docs/dev/directory/index.md",sourceDirName:"dev/directory",slug:"/dev/directory/",permalink:"/dev/directory/",draft:!1,editUrl:"https://github.com/biobankinguk/biobankinguk/edit/main/docs/docs/dev/directory/index.md",tags:[],version:"current",sidebarPosition:1,frontMatter:{sidebar_position:1},sidebar:"devGuide",previous:{title:"users",permalink:"/dev/cli/users"},next:{title:"Elasticsearch",permalink:"/dev/directory/elasticsearch"}},p={},c=[{value:"Setup",id:"setup",level:2},{value:"Optional steps",id:"optional-steps",level:2}],s={toc:c},u="wrapper";function d(e){let{components:t,...r}=e;return(0,a.kt)(u,(0,n.Z)({},s,r,{components:t,mdxType:"MDXLayout"}),(0,a.kt)("h1",{id:"overview"},"Overview"),(0,a.kt)("p",null,"The Directory project is the core piece of the BiobankingUK stack. Everything else in the stack serves to optionally augment the Directory."),(0,a.kt)("p",null,"It consists of an .NET Core MVC web application, backed by a PostgreSQL database, which interacts with an Elasticsearch server."),(0,a.kt)("h2",{id:"setup"},"Setup"),(0,a.kt)("blockquote",null,(0,a.kt)("p",{parentName:"blockquote"},"\u2139 Complete the instructions in ",(0,a.kt)("a",{parentName:"p",href:"getting-started"},"Getting Started")," first.")),(0,a.kt)("ol",null,(0,a.kt)("li",{parentName:"ol"},(0,a.kt)("p",{parentName:"li"},"Enable ",(0,a.kt)("a",{parentName:"p",href:"https://nodejs.org/api/corepack.html"},"Corepack")),(0,a.kt)("ul",{parentName:"li"},(0,a.kt)("li",{parentName:"ul"},"Simply run ",(0,a.kt)("inlineCode",{parentName:"li"},"corepack enable")," in your cli"))),(0,a.kt)("li",{parentName:"ol"},(0,a.kt)("p",{parentName:"li"},"Install Node Packages"),(0,a.kt)("ul",{parentName:"li"},(0,a.kt)("li",{parentName:"ul"},"Run ",(0,a.kt)("inlineCode",{parentName:"li"},"pnpm i")," from the project root."))),(0,a.kt)("li",{parentName:"ol"},(0,a.kt)("p",{parentName:"li"},"Add a new user"),(0,a.kt)("ul",{parentName:"li"},(0,a.kt)("li",{parentName:"ul"},"Change directory next to the ",(0,a.kt)("inlineCode",{parentName:"li"},"Api.csproj")," file."),(0,a.kt)("li",{parentName:"ul"},"Run ",(0,a.kt)("inlineCode",{parentName:"li"},"dotnet run -- users add <EMAIL> <FULL-NAME> -r <ROLES> -p <PASSWORD>")),(0,a.kt)("li",{parentName:"ul"},"For example: ",(0,a.kt)("inlineCode",{parentName:"li"},"dotnet run -- users add admin@example.com Admin -r SuperUser DirectoryAdmin -p Password1!")),(0,a.kt)("li",{parentName:"ul"},"For local dev use you probably want the roles: ",(0,a.kt)("inlineCode",{parentName:"li"},"SuperUser"),", and ",(0,a.kt)("inlineCode",{parentName:"li"},"DirectoryAdmin"),(0,a.kt)("ul",{parentName:"li"},(0,a.kt)("li",{parentName:"ul"},"being a ",(0,a.kt)("inlineCode",{parentName:"li"},"SuperUser")," doesn't automatically put you in the ",(0,a.kt)("inlineCode",{parentName:"li"},"DirectoryAdmin")," role too. Sorry."))),(0,a.kt)("li",{parentName:"ul"},"See ",(0,a.kt)("a",{parentName:"li",href:"cli/users#add"},"users CLI documentation")))),(0,a.kt)("li",{parentName:"ol"},(0,a.kt)("p",{parentName:"li"},"Check Email Configuration"),(0,a.kt)("ul",{parentName:"li"},(0,a.kt)("li",{parentName:"ul"},"by default for local development, the app will write emails to ",(0,a.kt)("inlineCode",{parentName:"li"},"~/temp")),(0,a.kt)("li",{parentName:"ul"},"See ",(0,a.kt)("a",{parentName:"li",href:"directory/email-sending"},"Email Sending"))))),(0,a.kt)("h2",{id:"optional-steps"},"Optional steps"),(0,a.kt)("p",null,"To use the Search functionality:"),(0,a.kt)("ol",null,(0,a.kt)("li",{parentName:"ol"},"Setup a local Elasticsearch instance",(0,a.kt)("ul",{parentName:"li"},(0,a.kt)("li",{parentName:"ul"},"See ",(0,a.kt)("a",{parentName:"li",href:"directory/elasticsearch"},"Elasticsearch"),", Docker recommended")))))}d.isMDXComponent=!0}}]);
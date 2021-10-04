"use strict";(self.webpackChunkdocs=self.webpackChunkdocs||[]).push([[468],{4852:function(e,t,n){n.d(t,{Zo:function(){return p},kt:function(){return m}});var i=n(9231);function r(e,t,n){return t in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function a(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var i=Object.getOwnPropertySymbols(e);t&&(i=i.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,i)}return n}function o(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?a(Object(n),!0).forEach((function(t){r(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):a(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function s(e,t){if(null==e)return{};var n,i,r=function(e,t){if(null==e)return{};var n,i,r={},a=Object.keys(e);for(i=0;i<a.length;i++)n=a[i],t.indexOf(n)>=0||(r[n]=e[n]);return r}(e,t);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);for(i=0;i<a.length;i++)n=a[i],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(r[n]=e[n])}return r}var l=i.createContext({}),u=function(e){var t=i.useContext(l),n=t;return e&&(n="function"==typeof e?e(t):o(o({},t),e)),n},p=function(e){var t=u(e.components);return i.createElement(l.Provider,{value:t},e.children)},c={inlineCode:"code",wrapper:function(e){var t=e.children;return i.createElement(i.Fragment,{},t)}},d=i.forwardRef((function(e,t){var n=e.components,r=e.mdxType,a=e.originalType,l=e.parentName,p=s(e,["components","mdxType","originalType","parentName"]),d=u(n),m=r,h=d["".concat(l,".").concat(m)]||d[m]||c[m]||a;return n?i.createElement(h,o(o({ref:t},p),{},{components:n})):i.createElement(h,o({ref:t},p))}));function m(e,t){var n=arguments,r=t&&t.mdxType;if("string"==typeof e||r){var a=n.length,o=new Array(a);o[0]=d;var s={};for(var l in t)hasOwnProperty.call(t,l)&&(s[l]=t[l]);s.originalType=e,s.mdxType="string"==typeof e?e:r,o[1]=s;for(var u=2;u<a;u++)o[u]=n[u];return i.createElement.apply(null,o)}return i.createElement.apply(null,n)}d.displayName="MDXCreateElement"},9906:function(e,t,n){n.r(t),n.d(t,{contentTitle:function(){return m},default:function(){return k},frontMatter:function(){return d},metadata:function(){return h},toc:function(){return f}});var i=n(8500),r=n(1304),a=n(9231),o=n(4852),s=n(1506),l="tableOfContentsInline_2XFb";function u(e){var t=e.toc,n=e.isChild;return t.length?a.createElement("ul",{className:n?"":"table-of-contents"},t.map((function(e){return a.createElement("li",{key:e.id},a.createElement("a",{href:"#"+e.id,dangerouslySetInnerHTML:{__html:e.value}}),a.createElement(u,{isChild:!0,toc:e.children}))}))):null}var p=function(e){var t=e.toc;return a.createElement("div",{className:(0,s.Z)(l)},a.createElement(u,{toc:t}))},c=["components"],d={title:"Process Overview",sidebar_position:1},m=void 0,h={unversionedId:"api-guide/bulk-submissions/process-overview",id:"api-guide/bulk-submissions/process-overview",isDocsHomePage:!1,title:"Process Overview",description:"Typically, data is submitted to the Directory manually, by Organisation Administrators through forms in the web application.",source:"@site/docs/api-guide/bulk-submissions/process-overview.mdx",sourceDirName:"api-guide/bulk-submissions",slug:"/api-guide/bulk-submissions/process-overview",permalink:"/api-guide/bulk-submissions/process-overview",editUrl:"https://github.com/biobankinguk/biobankinguk/edit/main/docs/docs/api-guide/bulk-submissions/process-overview.mdx",tags:[],version:"current",sidebarPosition:1,frontMatter:{title:"Process Overview",sidebar_position:1},sidebar:"apiGuide",previous:{title:"Authentication",permalink:"/api-guide/getting-started/authentication"},next:{title:"Examples",permalink:"/api-guide/bulk-submissions/examples"}},f=[{value:"Prerequisites",id:"prerequisites",children:[]},{value:"1. Submitting samples",id:"1-submitting-samples",children:[{value:"Operations",id:"operations",children:[]},{value:"Submission Record Types",id:"submission-record-types",children:[]},{value:"Sample records",id:"sample-records",children:[]}]}],b={toc:f};function k(e){var t=e.components,n=(0,r.Z)(e,c);return(0,o.kt)("wrapper",(0,i.Z)({},b,n,{components:t,mdxType:"MDXLayout"}),(0,o.kt)("p",null,"Typically, data is submitted to the Directory manually, by Organisation Administrators through forms in the web application."),(0,o.kt)("p",null,"However for many Organisations, this is an unrealistic expectation,\ndue to the volume of data and the need to aggregate individual sample records into an anonymised, summarised ",(0,o.kt)("strong",{parentName:"p"},"Collection"),"."),(0,o.kt)("p",null,"The Directory API provides a set of HTTP endpoints and a process for submitting sample records in Bulk,\nand the Directory will take care of the aggregation and surfacing of data via Search."),(0,o.kt)(p,{toc:f,mdxType:"TOCInline"}),(0,o.kt)("h2",{id:"prerequisites"},"Prerequisites"),(0,o.kt)("p",null,"All Bulk Submission endpoints described here require ",(0,o.kt)("strong",{parentName:"p"},"Token Authentication"),"."),(0,o.kt)("p",null,"This process guide assumes you have a token and will use it to authenticate all requests."),(0,o.kt)("p",null,"Getting a token in turn requires ",(0,o.kt)("strong",{parentName:"p"},"Client Credentials")," for an ",(0,o.kt)("strong",{parentName:"p"},"Organisation"),"."),(0,o.kt)("ul",null,(0,o.kt)("li",{parentName:"ul"},"Generating Client Credentials is covered in the ",(0,o.kt)("a",{parentName:"li",href:"../../directory-guide/bulk-submissions/api-credentials"},"Directory Guide")),(0,o.kt)("li",{parentName:"ul"},"Authenticating with Basic Auth (for the ",(0,o.kt)("inlineCode",{parentName:"li"},"/token")," endpoint) is described in the ",(0,o.kt)("a",{parentName:"li",href:"../getting-started/authentication"},"Authentication")," documentation."),(0,o.kt)("li",{parentName:"ul"},"Authenticating with an access token is described in the ",(0,o.kt)("a",{parentName:"li",href:"../getting-started/authentication"},"Authentication")," documentation.")),(0,o.kt)("div",{className:"admonition admonition-info alert alert--info"},(0,o.kt)("div",{parentName:"div",className:"admonition-heading"},(0,o.kt)("h5",{parentName:"div"},(0,o.kt)("span",{parentName:"h5",className:"admonition-icon"},(0,o.kt)("svg",{parentName:"span",xmlns:"http://www.w3.org/2000/svg",width:"14",height:"16",viewBox:"0 0 14 16"},(0,o.kt)("path",{parentName:"svg",fillRule:"evenodd",d:"M7 2.3c3.14 0 5.7 2.56 5.7 5.7s-2.56 5.7-5.7 5.7A5.71 5.71 0 0 1 1.3 8c0-3.14 2.56-5.7 5.7-5.7zM7 1C3.14 1 0 4.14 0 8s3.14 7 7 7 7-3.14 7-7-3.14-7-7-7zm1 3H6v5h2V4zm0 6H6v2h2v-2z"}))),"info")),(0,o.kt)("div",{parentName:"div",className:"admonition-content"},(0,o.kt)("p",{parentName:"div"},"Tokens have a short lifetime (e.g. 1 day), but last longer than a single\nrequest, so a client can cache and reuse a token until it expires, and only\nthen request a new one."))),(0,o.kt)("h2",{id:"1-submitting-samples"},"1. Submitting samples"),(0,o.kt)("p",null,"The first step of the Bulk Submissions process is to submit the sample record data to a staging area."),(0,o.kt)("p",null,"Submissions are made at the ",(0,o.kt)("inlineCode",{parentName:"p"},"/submit")," endpoint, for a given Organisation Internal Identifier:"),(0,o.kt)("p",null,"HTTP POST: ",(0,o.kt)("inlineCode",{parentName:"p"},"https://<submissions-api>/submit/{id}")),(0,o.kt)("p",null,"The payload is detailed in the ",(0,o.kt)("a",{parentName:"p",href:"https://submissions.biobankinguk.org/"},"OpenAPI documentation"),",\nbut some less obvious aspects are described here."),(0,o.kt)("h3",{id:"operations"},"Operations"),(0,o.kt)("p",null,"Submission payloads can contain multiple ",(0,o.kt)("strong",{parentName:"p"},"Operations"),":"),(0,o.kt)("ul",null,(0,o.kt)("li",{parentName:"ul"},(0,o.kt)("strong",{parentName:"li"},"Operations")," are either ",(0,o.kt)("inlineCode",{parentName:"li"},"Submit")," or ",(0,o.kt)("inlineCode",{parentName:"li"},"Delete")),(0,o.kt)("li",{parentName:"ul"},(0,o.kt)("inlineCode",{parentName:"li"},"Submit")," will insert or update a record in the staging area",(0,o.kt)("ul",{parentName:"li"},(0,o.kt)("li",{parentName:"ul"},"when the staged data is committed, this record will be included in the live dataset"))),(0,o.kt)("li",{parentName:"ul"},(0,o.kt)("inlineCode",{parentName:"li"},"Delete")," will stage a deletion request for a record",(0,o.kt)("ul",{parentName:"li"},(0,o.kt)("li",{parentName:"ul"},"when the staged data is committed, this record will be deleted from the live dataset"))),(0,o.kt)("li",{parentName:"ul"},(0,o.kt)("inlineCode",{parentName:"li"},"Delete")," operations only need to specify the identifying properties of the record.\nThese are properties marked as ",(0,o.kt)("strong",{parentName:"li"},"required")," in the OpenAPI model."),(0,o.kt)("li",{parentName:"ul"},(0,o.kt)("inlineCode",{parentName:"li"},"Submit")," operations will have further requirements of mandatory properties.\nSome of these requirements are conditional on other property values. See below.")),(0,o.kt)("h3",{id:"submission-record-types"},"Submission Record Types"),(0,o.kt)("p",null,"There are three record types that can be contained in a Submission: Samples, Diagnoses and Treatments."),(0,o.kt)("p",null,"While the API will accept and correctly store diagnoses and treatments,\nthey are of no use at this time as the Directory only deals with samples."),(0,o.kt)("h3",{id:"sample-records"},"Sample records"))}k.isMDXComponent=!0},1506:function(e,t,n){function i(e){var t,n,r="";if("string"==typeof e||"number"==typeof e)r+=e;else if("object"==typeof e)if(Array.isArray(e))for(t=0;t<e.length;t++)e[t]&&(n=i(e[t]))&&(r&&(r+=" "),r+=n);else for(t in e)e[t]&&(r&&(r+=" "),r+=t);return r}function r(){for(var e,t,n=0,r="";n<arguments.length;)(e=arguments[n++])&&(t=i(e))&&(r&&(r+=" "),r+=t);return r}n.d(t,{Z:function(){return r}})}}]);
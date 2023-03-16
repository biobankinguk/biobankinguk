"use strict";(self.webpackChunkdocs=self.webpackChunkdocs||[]).push([[40],{8570:(e,t,n)=>{n.d(t,{Zo:()=>u,kt:()=>m});var a=n(79);function s(e,t,n){return t in e?Object.defineProperty(e,t,{value:n,enumerable:!0,configurable:!0,writable:!0}):e[t]=n,e}function i(e,t){var n=Object.keys(e);if(Object.getOwnPropertySymbols){var a=Object.getOwnPropertySymbols(e);t&&(a=a.filter((function(t){return Object.getOwnPropertyDescriptor(e,t).enumerable}))),n.push.apply(n,a)}return n}function o(e){for(var t=1;t<arguments.length;t++){var n=null!=arguments[t]?arguments[t]:{};t%2?i(Object(n),!0).forEach((function(t){s(e,t,n[t])})):Object.getOwnPropertyDescriptors?Object.defineProperties(e,Object.getOwnPropertyDescriptors(n)):i(Object(n)).forEach((function(t){Object.defineProperty(e,t,Object.getOwnPropertyDescriptor(n,t))}))}return e}function r(e,t){if(null==e)return{};var n,a,s=function(e,t){if(null==e)return{};var n,a,s={},i=Object.keys(e);for(a=0;a<i.length;a++)n=i[a],t.indexOf(n)>=0||(s[n]=e[n]);return s}(e,t);if(Object.getOwnPropertySymbols){var i=Object.getOwnPropertySymbols(e);for(a=0;a<i.length;a++)n=i[a],t.indexOf(n)>=0||Object.prototype.propertyIsEnumerable.call(e,n)&&(s[n]=e[n])}return s}var l=a.createContext({}),d=function(e){var t=a.useContext(l),n=t;return e&&(n="function"==typeof e?e(t):o(o({},t),e)),n},u=function(e){var t=d(e.components);return a.createElement(l.Provider,{value:t},e.children)},c="mdxType",p={inlineCode:"code",wrapper:function(e){var t=e.children;return a.createElement(a.Fragment,{},t)}},h=a.forwardRef((function(e,t){var n=e.components,s=e.mdxType,i=e.originalType,l=e.parentName,u=r(e,["components","mdxType","originalType","parentName"]),c=d(n),h=s,m=c["".concat(l,".").concat(h)]||c[h]||p[h]||i;return n?a.createElement(m,o(o({ref:t},u),{},{components:n})):a.createElement(m,o({ref:t},u))}));function m(e,t){var n=arguments,s=t&&t.mdxType;if("string"==typeof e||s){var i=n.length,o=new Array(i);o[0]=h;var r={};for(var l in t)hasOwnProperty.call(t,l)&&(r[l]=t[l]);r.originalType=e,r[c]="string"==typeof e?e:s,o[1]=r;for(var d=2;d<i;d++)o[d]=n[d];return a.createElement.apply(null,o)}return a.createElement.apply(null,n)}h.displayName="MDXCreateElement"},5952:(e,t,n)=>{n.r(t),n.d(t,{assets:()=>l,contentTitle:()=>o,default:()=>p,frontMatter:()=>i,metadata:()=>r,toc:()=>d});var a=n(5675),s=(n(79),n(8570));const i={sidebar_position:3},o="Seed Data",r={unversionedId:"dev/getting-started/seed-data",id:"dev/getting-started/seed-data",title:"Seed Data",description:"- This represent the tables you'd want to seed in a new installation",source:"@site/docs/dev/getting-started/seed-data.md",sourceDirName:"dev/getting-started",slug:"/dev/getting-started/seed-data",permalink:"/dev/getting-started/seed-data",draft:!1,editUrl:"https://github.com/biobankinguk/biobankinguk/edit/main/docs/docs/dev/getting-started/seed-data.md",tags:[],version:"current",sidebarPosition:3,frontMatter:{sidebar_position:3},sidebar:"devGuide",previous:{title:"Projects Structure",permalink:"/dev/getting-started/structure"},next:{title:"\ud83d\udca1 pnpm cheatsheet",permalink:"/dev/getting-started/pnpm-cheatsheet"}},l={},d=[{value:"Additional notes on pending RefData changes",id:"additional-notes-on-pending-refdata-changes",level:2},{value:"Diagnoses (SNOMED Terms)",id:"diagnoses-snomed-terms",level:3},{value:"Material Types",id:"material-types",level:3},{value:"Macroscopic Assessments",id:"macroscopic-assessments",level:3},{value:"Preservation Types",id:"preservation-types",level:3},{value:"Collection Percentages",id:"collection-percentages",level:3},{value:"Age Ranges",id:"age-ranges",level:3},{value:"Genders",id:"genders",level:3},{value:"Donor Counts",id:"donor-counts",level:3},{value:"Access Conditions",id:"access-conditions",level:3},{value:"Collection Types",id:"collection-types",level:3},{value:"Collection Statuses",id:"collection-statuses",level:3},{value:"Collection Points",id:"collection-points",level:3},{value:"Consent Restrictions",id:"consent-restrictions",level:3},{value:"Associated Data Types",id:"associated-data-types",level:3},{value:"Annual Statistics",id:"annual-statistics",level:3},{value:"SOP Statuses",id:"sop-statuses",level:3},{value:"Sample Collection Mode",id:"sample-collection-mode",level:3},{value:"Associated Data Procurement Timeframes",id:"associated-data-procurement-timeframes",level:3},{value:"HTA Statuses",id:"hta-statuses",level:3},{value:"Organisation Service Offerings",id:"organisation-service-offerings",level:3},{value:"Countries",id:"countries",level:3},{value:"Counties",id:"counties",level:3},{value:"Registration Reasons",id:"registration-reasons",level:3}],u={toc:d},c="wrapper";function p(e){let{components:t,...n}=e;return(0,s.kt)(c,(0,a.Z)({},u,n,{components:t,mdxType:"MDXLayout"}),(0,s.kt)("h1",{id:"seed-data"},"Seed Data"),(0,s.kt)("ul",null,(0,s.kt)("li",{parentName:"ul"},"This represent the tables you'd want to seed in a new installation"),(0,s.kt)("li",{parentName:"ul"},"All other tables are dynamic system or user data"),(0,s.kt)("li",{parentName:"ul"},"These sample records are based on Production at the time of writing"),(0,s.kt)("li",{parentName:"ul"},"Feel free to substitute your own data; these are samples"),(0,s.kt)("li",{parentName:"ul"},"In particular, curation of ",(0,s.kt)("inlineCode",{parentName:"li"},"MaterialTypes")," and ",(0,s.kt)("inlineCode",{parentName:"li"},"Diagnosis")," records is likely"),(0,s.kt)("li",{parentName:"ul"},"IDs typically shouldn't be imported, unless you feel the need to preserve them"),(0,s.kt)("li",{parentName:"ul"},"There are a few dependent entities (with FK constraints). You'll need to ",(0,s.kt)("inlineCode",{parentName:"li"},"INSERT")," the dependencies first and ensure the FK IDs line up.",(0,s.kt)("ul",{parentName:"li"},(0,s.kt)("li",{parentName:"ul"},(0,s.kt)("inlineCode",{parentName:"li"},"Counties")," depend on ",(0,s.kt)("inlineCode",{parentName:"li"},"Countries"),"."),(0,s.kt)("li",{parentName:"ul"},(0,s.kt)("inlineCode",{parentName:"li"},"AnnualStatistics")," depend on ",(0,s.kt)("inlineCode",{parentName:"li"},"AnnualStatisticGroups")))),(0,s.kt)("li",{parentName:"ul"},"Some tables may be redundant (e.g. ",(0,s.kt)("inlineCode",{parentName:"li"},"HtaStatus"),") but should be populated as expected until such time as they are properly removed from the codebase."),(0,s.kt)("li",{parentName:"ul"},(0,s.kt)("inlineCode",{parentName:"li"},"Funders")," weren't seeded by the old Directory seed process, but it makes sense to want to prepopulate a starting point for them in a new environment")),(0,s.kt)("p",null,"See ",(0,s.kt)("a",{parentName:"p",href:"/dev/cli/ref-data#seed"},(0,s.kt)("inlineCode",{parentName:"a"},"ref-data seed")," documentation"),"."),(0,s.kt)("h2",{id:"additional-notes-on-pending-refdata-changes"},"Additional notes on pending RefData changes"),(0,s.kt)("h3",{id:"diagnoses-snomed-terms"},"Diagnoses (SNOMED Terms)"),(0,s.kt)("p",null,"These should be seeded based on an existing environment, (especially since they are editable through the admin UI) or selected freshly from the full list of terms, if desirable for a new environment."),(0,s.kt)("p",null,"TODO: coming MIABIS and OMOP changes are likely to affect this notably including the move to the separate Core RefData Service"),(0,s.kt)("h3",{id:"material-types"},"Material Types"),(0,s.kt)("p",null,"These should be seeded based on an existing environment or selected freshly."),(0,s.kt)("p",null,"For new environments, ID's need not be preserved.\nWhen migrating environment data, ID's should be preserved, to maintain reign keys on the incoming data."),(0,s.kt)("p",null,"SortOrder should be alphabetical, and adjusted when new terms are added"),(0,s.kt)("h3",{id:"macroscopic-assessments"},"Macroscopic Assessments"),(0,s.kt)("p",null,"These should be seeded based on Core RefData Services Seed Data."),(0,s.kt)("h3",{id:"preservation-types"},"Preservation Types"),(0,s.kt)("p",null,'These should be seeded based on Core RefData Services Seed Data.\nCalled "Storage Temperatures" in the new RefData.'),(0,s.kt)("h3",{id:"collection-percentages"},"Collection Percentages"),(0,s.kt)("p",null,"These should be seeded based on an existing environment or example dataset for now."),(0,s.kt)("h3",{id:"age-ranges"},"Age Ranges"),(0,s.kt)("p",null,"These should be seeded based on Core RefData Services Seed Data."),(0,s.kt)("h3",{id:"genders"},"Genders"),(0,s.kt)("p",null,'These should be seeded based on an existing environment or example dataset for now\nCalled "Sex" in Core RefData Service.'),(0,s.kt)("h3",{id:"donor-counts"},"Donor Counts"),(0,s.kt)("p",null,"These should be seeded based on Core RefData Services Seed Data."),(0,s.kt)("h3",{id:"access-conditions"},"Access Conditions"),(0,s.kt)("p",null,"These should be seeded based on Core RefData Services Seed Data."),(0,s.kt)("h3",{id:"collection-types"},"Collection Types"),(0,s.kt)("p",null,"These should be seeded based on Core RefData Services Seed Data."),(0,s.kt)("h3",{id:"collection-statuses"},"Collection Statuses"),(0,s.kt)("p",null,"These should be seeded based on Core RefData Services Seed Data."),(0,s.kt)("h3",{id:"collection-points"},"Collection Points"),(0,s.kt)("p",null,"These should be seeded based on Core RefData Services Seed Data."),(0,s.kt)("h3",{id:"consent-restrictions"},"Consent Restrictions"),(0,s.kt)("p",null,"These should be seeded based on Core RefData Services Seed Data."),(0,s.kt)("h3",{id:"associated-data-types"},"Associated Data Types"),(0,s.kt)("p",null,"These should be seeded based on Core RefData Services Seed Data."),(0,s.kt)("h3",{id:"annual-statistics"},"Annual Statistics"),(0,s.kt)("p",null,'These should be seeded based on Core RefData Services Seed Data.\nFirst "Annual Statistic Group", then "Annual Statistic" with foreign keys to oups'),(0,s.kt)("h3",{id:"sop-statuses"},"SOP Statuses"),(0,s.kt)("p",null,"These should be seeded based on Core RefData Services Seed Data."),(0,s.kt)("h3",{id:"sample-collection-mode"},"Sample Collection Mode"),(0,s.kt)("p",null,"These should be seeded based on an existing environment or example dataset for now."),(0,s.kt)("h3",{id:"associated-data-procurement-timeframes"},"Associated Data Procurement Timeframes"),(0,s.kt)("p",null,"These should be seeded based on Core RefData Services Seed Data."),(0,s.kt)("h3",{id:"hta-statuses"},"HTA Statuses"),(0,s.kt)("p",null,"These should be seeded based on Core RefData Services Seed Data."),(0,s.kt)("h3",{id:"organisation-service-offerings"},"Organisation Service Offerings"),(0,s.kt)("p",null,'These should be seeded based on Core RefData Services Seed Data.\nCalled "Service Offerings".'),(0,s.kt)("h3",{id:"countries"},"Countries"),(0,s.kt)("p",null,"These should be seeded based on Core RefData Services Seed Data."),(0,s.kt)("h3",{id:"counties"},"Counties"),(0,s.kt)("p",null,"These should be seeded based on Core RefData Services Seed Data."),(0,s.kt)("h3",{id:"registration-reasons"},"Registration Reasons"),(0,s.kt)("p",null,"These should be seeded based on an existing environment or example dataset for now."))}p.isMDXComponent=!0}}]);
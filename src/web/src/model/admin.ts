import { ReactNode } from "react";

export interface TabModel{
    key:string;
    title:string;
    component:ReactNode,
}

export interface TokenModel{
    token:string,
    expire:string
}
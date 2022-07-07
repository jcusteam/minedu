export interface IGatewayResponse<T> {
    success: boolean;
    data: T;
    messages: string[];
    messageType: string;
}

export interface IStatusResponse<T> {
    success: boolean;
    data: T;
    messages: string[];
    messageType: string;
}

export interface IStatusResponse<T> {
    success: boolean;
    data: T;
    messages: string[];
    messageType: string;
}

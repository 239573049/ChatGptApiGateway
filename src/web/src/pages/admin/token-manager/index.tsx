import { Table, Button, Notification, Modal, DatePicker, Input } from "@douyinfe/semi-ui";
import { Component } from "react";
import { IconDelete } from '@douyinfe/semi-icons';
import { TokenModel } from "../../../model/admin";
import api from "../../../utils/request";

interface IProps {

}

interface IState {
    data: TokenModel[],
    columns: any[],
    visible:boolean,
    value:TokenModel,
}

export default class TokenManager extends Component<IProps, IState> {
    state: IState = {
        data: [],
        columns: [
            {
                title: 'Token',
                dataIndex: 'token',
            },
            {
                title: '到期时间',
                dataIndex: 'expire',
            },
            {
                title: '',
                dataIndex: 'operate',
                render: (value: string, data: TokenModel) => {
                    return (
                        <Button icon={<IconDelete />} theme="borderless" onClick={() => this.removeToken(data)} />
                    );
                },
            }
        ],
        visible: false,
        value: {
            token: '',
            expire: '2023-5-01 00:00:00'
        }
    }

    handleOk(){
        var {value,data,visible} = this.state;
        data.push(value);
        api.post("api/v1/Loggers", data)
            .then(res => {
                Notification.success({ content: '新增', title: '提示' });
                this.getList()
                this.setState({
                    visible:false
                })
            });
        this.setState({ data ,value:{
            token: '',
            expire: '2023-5-01 00:00:00'
        }});
    }

    removeToken(token: TokenModel) {
        // 清除集合的数据
        var { data } = this.state;
        var index = data.findIndex(item => item.token === token.token);
        if (index !== -1) {
            data.splice(index, 1);
        }
        api.post("api/v1/Loggers", data)
            .then(res => {
                Notification.success({ content: '删除成功', title: '提示' });
            });
        this.setState({ data });
        this.getList()
    }

    getList(){
        api.get('api/v1/Loggers/List')
            .then(res => {
                console.log(res.data);
                this.setState({
                    data: res.data
                })
            });
    }

    componentDidMount() {
        this.getList()
    }


    render() {
        var { data, value,columns } = this.state;
        return (<div>
            <div>
                <Button theme='solid' type='primary' style={{ marginRight: 8 }} onClick={()=>this.setState({
                    visible:true
                })}>新增token</Button>
            </div>
            <Table columns={columns} dataSource={data} pagination={false} />
            <Modal
                title="自定义样式"
                visible={this.state.visible}
                onOk={()=>this.handleOk()}
                onCancel={()=>this.setState({
                    visible:false
                })}
                centered
                bodyStyle={{ overflow: 'auto', height: 200 }}
            >
                <div>
                    <Input value={value.token} onChange={(v)=>{
                            value.token=v;
                            this.setState({value})
                        }} defaultValue='token'></Input>
                </div>
                <div>
                    <div style={{ textAlign: 'center', margin: 5 }}>过期时间</div>
                    <DatePicker value={value.expire} onChange={(a:any,dateStr:any)=>{
                        value.expire = dateStr;
                        this.setState({
                            value
                        })
                    }} type="dateTime" density="compact" />
                </div>
            </Modal>
        </div>)
    }
}
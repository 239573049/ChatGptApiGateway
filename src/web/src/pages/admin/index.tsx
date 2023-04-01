import { Layout, Nav, Button, Breadcrumb, Skeleton, Avatar, Tabs, TabPane } from '@douyinfe/semi-ui';
import { IconBell, IconHelpCircle, IconBytedanceLogo, IconHome, IconHistogram, IconLive,IconClose } from '@douyinfe/semi-icons';
import { Component, ReactNode } from "react";
import './index.css'
import { TabModel } from '../../model/admin';
import Home from './home';
import Logger from './logger';
import TokenManager from './token-manager';

const { Header, Footer, Sider, Content } = Layout;

interface IProps {

}

interface IState {
    tabs: TabModel[],
    selectKey: string
}

export default class Admin extends Component<IProps, IState> {
    state: IState = {
        tabs: [{
            title: '首页',
            key: 'home',
            component: <Home />
        }],
        selectKey: 'Home'
    }
    constructor(props: any) {
        super(props)
        var token = window.localStorage.getItem('token');
        if (!token) {
            window.open('/')
        }
    }
    addTab(tab: TabModel) {
        var { tabs, selectKey } = this.state;
        var index = tabs.findIndex(item => item.key === tab.key);
        if (index === -1) {
            selectKey = tab.key;
            tabs.push(tab)
        } else {
            selectKey = tabs[index].key;
        }
        this.setState({ tabs, selectKey })
    }

    handleChange(selectKey:string) {
        this.setState({ selectKey });
    }

    handleClose(selectKey:string){
        // 删除数组
        var { tabs } = this.state;
        if(tabs.length===1){
            return;
        }
        var index = tabs.findIndex(item => item.key === selectKey);
        if (index !== -1) {
            tabs.splice(index, 1);
        }
        this.setState({ tabs, selectKey: tabs[0].key });
    }

    render(): ReactNode {
        var { tabs, selectKey } = this.state;
        return (
            <Layout style={{ border: '1px solid var(--semi-color-border)' }}>
                <Sider style={{ backgroundColor: 'var(--semi-color-bg-1)' }}>
                    <Nav
                        defaultSelectedKeys={[selectKey]}
                        style={{ maxWidth: 220, height: '100%' }}
                        items={[
                            { itemKey: 'home', text: '首页', icon: <IconHome size="large" />, 
                            onClick: () => { this.addTab({ key: 'home', component: <Home />, title: "首页" }) } },
                            { itemKey: 'logger', text: '日志数据', icon: <IconHistogram size="large" />, 
                            onClick: () => { this.addTab({ key: 'logger', component: <Logger />, title: "日志数据" }) } },
                            { itemKey: 'tokenManager', text: 'token管理', icon: <IconLive size="large" />, 
                            onClick: () => { this.addTab({ key: 'tokenManager', component: <TokenManager />, title: "token管理" }) } },
                        ]}
                        header={{
                            logo: <img src="//lf1-cdn-tos.bytescm.com/obj/ttfe/ies/semi/webcast_logo.svg" />,
                            text: 'ApiGateway',
                        }}
                        footer={{
                            collapseButton: true,
                        }}
                    />
                </Sider>
                <Layout>
                    <Header style={{ backgroundColor: 'var(--semi-color-bg-1)' }}>
                        <Nav
                            mode="horizontal"
                            footer={
                                <>
                                    <Avatar color="orange" size="small">
                                        Token
                                    </Avatar>
                                </>
                            }
                        ></Nav>
                    </Header>
                    <Content
                        style={{
                            padding: '24px',
                            backgroundColor: 'var(--semi-color-bg-0)',
                        }}
                    >
                        <Tabs
                            defaultActiveKey="home"
                            activeKey={selectKey}
                            type="card"
                            style={{ height: '100%' }}
                            onTabClose={(key)=>this.handleClose(key)}
                            onChange={(key)=>this.handleChange(key)}
                        >
                            {tabs.map((t: TabModel) => (
                                <TabPane style={{height:"100%"}} tab={<div>{t.title}<IconClose onClick={()=>this.handleClose(t.key)}/></div>} itemKey={t.key} key={t.key}>
                                    {t.component}
                                </TabPane>
                            ))}
                        </Tabs>
                    </Content>
                    <Footer
                        style={{
                            display: 'flex',
                            justifyContent: 'space-between',
                            padding: '20px',
                            color: 'var(--semi-color-text-2)',
                            backgroundColor: 'rgba(var(--semi-grey-0), 1)',
                        }}
                    >
                        <span
                            style={{
                                display: 'flex',
                                alignItems: 'center',
                            }}
                        >
                            <IconBytedanceLogo size="large" style={{ marginRight: '8px' }} />
                            <span>版权所有@2023 Token。版权所有</span>
                        </span>
                        <span>
                            <span style={{ marginRight: '24px' }}>平台客服</span>
                            <span>反馈建议</span>
                        </span>
                    </Footer>
                </Layout>
            </Layout>
        )
    }
}
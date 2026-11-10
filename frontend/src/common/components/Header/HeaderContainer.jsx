import clsx from 'clsx';
import styles from './Header.module.scss';

const HeaderContainer = ({ children, className = "", style }) => {
    return (
      <div className={clsx(styles.headerContainer, className)} style={style}>
        {children}
      </div>
    );
  };
  
  export default HeaderContainer;
  